using Microsoft.EntityFrameworkCore;
using VetClinic.Application.Customers;
using VetClinic.Domain.Customers;

namespace VetClinic.Infrastructure.Persistence.Customers;

internal sealed class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public Task<bool> ExistsByCpf(Cpf cpf, CancellationToken cancellationToken = default) =>
        db.Customers.AnyAsync(c => c.Cpf == cpf, cancellationToken);

    public Task<bool> ExistsByEmail(Email email, CancellationToken cancellationToken = default) =>
        db.Customers.AnyAsync(c => c.Email == email, cancellationToken);

    public async Task Add(Customer customer, CancellationToken cancellationToken = default) =>
        await db.Customers.AddAsync(customer, cancellationToken);
}