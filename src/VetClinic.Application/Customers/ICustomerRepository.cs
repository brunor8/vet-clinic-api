using VetClinic.Domain.Customers;

namespace VetClinic.Application.Customers;

public interface ICustomerRepository
{
    Task<bool> ExistsByCpf(Cpf cpf, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmail(Email email, CancellationToken cancellationToken = default);
    Task Add(Customer customer, CancellationToken cancellationToken = default);
}