using System.Net.Mail;
using VetClinic.Domain.Common;

namespace VetClinic.Domain.Customers;

public sealed record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("E-mail é obrigatório.");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > 254
            || !MailAddress.TryCreate(normalized, out var parsed)
            || parsed.Address != normalized)
            throw new DomainException("E-mail inválido.");

        return new Email(normalized);
    }

    public override string ToString() => Value;
}