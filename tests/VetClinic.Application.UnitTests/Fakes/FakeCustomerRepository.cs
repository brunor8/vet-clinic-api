using VetClinic.Application.Customers;
using VetClinic.Domain.Customers;

namespace VetClinic.Application.UnitTests.Fakes;

public sealed class FakeCustomerRepository : ICustomerRepository
{
    public List<Customer> Customers { get; } = [];

    public Task<bool> ExistsByCpf(Cpf cpf, CancellationToken cancellationToken = default) =>
        Task.FromResult(Customers.Any(c => c.Cpf == cpf));

    public Task<bool> ExistsByEmail(Email email, CancellationToken cancellationToken = default) =>
        Task.FromResult(Customers.Any(c => c.Email == email));

    public Task Add(Customer customer, CancellationToken cancellationToken = default)
    {
        Customers.Add(customer);
        return Task.CompletedTask;
    }
}