namespace VetClinic.Application.Common.Results;

public sealed class Result<T>
{
    private readonly T? _value;
    private Result(T value)
    {
        _value = value;
        Errors = [];
    }

    private Result(IReadOnlyList<Error> errors) => Errors = errors;

    public bool IsSuccess => Errors.Count == 0;
    public IReadOnlyList<Error> Errors { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new([error]);
    public static Result<T> Failure(IReadOnlyList<Error> errors) => new(errors);

}