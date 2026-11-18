using VetClinic.Application.Common.Results;
using VetClinic.Domain.Common;
using VetClinic.Domain.Customers;

namespace VetClinic.Application.Customers.RegisterCustomer;

public static class RegisterCustomerValidator
{
    public static IReadOnlyList<Error> Validate(RegisterCustomerCommand command)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(command.Name))
            errors.Add(Error.Validation("name", "Nome é obrigatório."));
        else if (command.Name.Trim().Length > Customer.NameMaxLength)
            errors.Add(Error.Validation("name", $"Nome deve ter no máximo {Customer.NameMaxLength} caracteres."));

        Capture(errors, "cpf", () => Cpf.Create(command.Cpf));
        Capture(errors, "email", () => Email.Create(command.Email));
        Capture(errors, "phone", () => Phone.Create(command.Phone));

        return errors;
    }

    private static void Capture(List<Error> errors, string field, Action create)
    {
        try
        {
            create();
        }
        catch (DomainException ex)
        {
            errors.Add(Error.Validation(field, ex.Message));
        }
    }
}