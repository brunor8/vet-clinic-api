using VetClinic.Application.Common.Abstractions;
using VetClinic.Application.Common.Messaging;
using VetClinic.Application.Common.Results;
using VetClinic.Domain.Customers;

namespace VetClinic.Application.Customers.RegisterCustomer;

public sealed class RegisterCustomerHandler(
    ICustomerRepository customers,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<RegisterCustomerCommand, Result<Guid>>
{
    private static readonly Error CpfAlreadyRegistered =
        Error.Conflict("customer.cpf_already_registered", "Já existe um cliente com este CPF.");

    private static readonly Error EmailAlreadyRegistered =
        Error.Conflict("customer.email_already_registered", "Já existe um cliente com este e-mail.");

    public async Task<Result<Guid>> Handle(RegisterCustomerCommand command, CancellationToken cancellationToken)
    {
        var validationErrors = RegisterCustomerValidator.Validate(command);
        if (validationErrors.Count > 0)
            return Result<Guid>.Failure(validationErrors);

        var cpf = Cpf.Create(command.Cpf);
        var email = Email.Create(command.Email);
        var phone = Phone.Create(command.Phone);

        var conflicts = new List<Error>();

        if (await customers.ExistsByCpf(cpf, cancellationToken))
            conflicts.Add(CpfAlreadyRegistered);

        if (await customers.ExistsByEmail(email, cancellationToken))
            conflicts.Add(EmailAlreadyRegistered);

        if (conflicts.Count > 0)
            return Result<Guid>.Failure(conflicts);

        var customer = Customer.Register(command.Name, cpf, email, phone, timeProvider.GetUtcNow());

        await customers.Add(customer, cancellationToken);

        try
        {
            await unitOfWork.SaveChanges(cancellationToken);
        }
        catch (UniqueConstraintViolationException ex)
            when (ex.ConstraintName is CustomerConstraints.Cpf or CustomerConstraints.Email)
        {
            // Outra requisição gravou o mesmo CPF/e-mail entre a verificação e o save.
            return Result<Guid>.Failure(
                ex.ConstraintName == CustomerConstraints.Cpf ? CpfAlreadyRegistered : EmailAlreadyRegistered);
        }

        return Result<Guid>.Success(customer.Id);
    }
}