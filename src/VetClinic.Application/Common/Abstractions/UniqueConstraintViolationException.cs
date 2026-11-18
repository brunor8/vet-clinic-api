namespace VetClinic.Application.Common.Abstractions;

public sealed class UniqueConstraintViolationException(string? constraintName, Exception? innerException = null)
    : Exception($"Violação de unicidade: {constraintName}", innerException)
{
    public string? ConstraintName { get; } = constraintName;
}