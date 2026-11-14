using VetClinic.Domain.Common;

namespace VetClinic.Domain.Customers;

public sealed record Phone
{
    public string Value { get; }

    private Phone(string value) => Value = value;

    public static Phone Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Telefone é obrigatório.");

        var digits = new string(value.Where(char.IsDigit).ToArray());

        // 10-11 digits (DDD + number) or 12-13 with Brazil country code (55)
        if (digits.Length is < 10 or > 13)
            throw new DomainException("Telefone inválido.");

        return new Phone(digits);
    }

    public override string ToString() => Value;
}