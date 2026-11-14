using VetClinic.Domain.Common;

namespace VetClinic.Domain.Customers;

public sealed record Cpf
{
    public const int Length = 11;

    public string Value { get; }

    private Cpf(string value) => Value = value;

    public static Cpf Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("CPF é obrigatório.");

        var digits = new string(value.Where(char.IsDigit).ToArray());

        if (!IsValid(digits))
            throw new DomainException("CPF inválido.");

        return new Cpf(digits);
    }

    // 123.456.789-09
    public string Formatted() =>
        $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}";

    public override string ToString() => Value;

    private static bool IsValid(string digits)
    {
        if (digits.Length != Length)
            return false;

        // Reject repeated sequences (000.000.000-00, 111.111.111-11...),
        // that pass the digit calculation but are not valid CPFs.
        if (digits.Distinct().Count() == 1)
            return false;

        var firstCheck = CalculateCheckDigit(digits, length: 9);
        var secondCheck = CalculateCheckDigit(digits, length: 10);

        return digits[9] - '0' == firstCheck
            && digits[10] - '0' == secondCheck;
    }

    // Decreasing Weights: 10..2 for the 1st digit, 11..2 for the 2nd.
    private static int CalculateCheckDigit(string digits, int length)
    {
        var sum = 0;
        for (var i = 0; i < length; i++)
            sum += (digits[i] - '0') * (length + 1 - i);

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}