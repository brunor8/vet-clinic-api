using VetClinic.Domain.Common;

namespace VetClinic.Domain.Customers;

public sealed class Customer : Entity
{
    public const int NameMaxLength = 150;

    public string Name { get; private set; } = null;
    public Cpf Cpf { get; private set; } = null!;
    public Email Email { get; private set; } = null;
    public Phone Phone { get; private set; } = null;
    public bool IsActive { get; private set; } 
    public DateTimeOffset CreatedAt { get; private set; } 

    // EF Core
    private Customer() { }

    public static Customer Register(string? name, Cpf cpf, Email email, Phone phone, DateTimeOffset createdAt)
    {
        return new Customer
        {
            Name = ValidateName(name),
            Cpf = cpf,
            Email = email,
            Phone = phone,
            IsActive = true,
            CreatedAt = createdAt
        };
    }

    public void UpdateContact(string? name, Email email, Phone phone)
    {
        Name = ValidateName(name);
        Email = email;
        Phone = phone;
    }

    public void Deactivate() => IsActive = false;

    private static string ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");

        var trimmed = name.Trim();

        if (trimmed.Length > NameMaxLength)
            throw new DomainException($"Nome deve ter no máximo {NameMaxLength} caracteres.");

        return trimmed;
    }
}