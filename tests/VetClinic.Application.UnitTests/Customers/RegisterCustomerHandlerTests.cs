using VetClinic.Application.Common.Results;
using VetClinic.Application.Customers.RegisterCustomer;
using VetClinic.Application.UnitTests.Fakes;
using VetClinic.Domain.Customers;

namespace VetClinic.Application.UnitTests.Customers;

public class RegisterCustomerHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 14, 12, 0, 0, TimeSpan.Zero);

    private readonly FakeCustomerRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly RegisterCustomerHandler _handler;

    public RegisterCustomerHandlerTests()
    {
        _handler = new RegisterCustomerHandler(_repository, _unitOfWork, new FakeTimeProvider(Now));
    }

    private static RegisterCustomerCommand ValidCommand() =>
        new("Ana Silva", "529.982.247-25", "ana@example.com", "(11) 99999-9999");

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSucceedAndReturnId()
    {
        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistCustomerWithNormalizedData()
    {
        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        var customer = Assert.Single(_repository.Customers);
        Assert.Equal(result.Value, customer.Id);
        Assert.Equal("Ana Silva", customer.Name);
        Assert.Equal("52998224725", customer.Cpf.Value);
        Assert.Equal("ana@example.com", customer.Email.Value);
        Assert.Equal("11999999999", customer.Phone.Value);
        Assert.True(customer.IsActive);
        Assert.Equal(Now, customer.CreatedAt);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSaveChangesOnce()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.Equal(1, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldReturnValidationErrorsAndNotPersist()
    {
        var command = new RegisterCustomerCommand("", "123", "x", "1");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(4, result.Errors.Count);
        Assert.All(result.Errors, e => Assert.Equal(ErrorType.Validation, e.Type));
        Assert.Empty(_repository.Customers);
        Assert.Equal(0, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WithDuplicateCpf_ShouldReturnConflictAndNotPersist()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        var second = ValidCommand() with { Email = "outro@example.com" };
        var result = await _handler.Handle(second, CancellationToken.None);

        var error = Assert.Single(result.Errors);
        Assert.Equal("customer.cpf_already_registered", error.Code);
        Assert.Equal(ErrorType.Conflict, error.Type);
        Assert.Single(_repository.Customers);
        Assert.Equal(1, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldReturnConflictAndNotPersist()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        var second = ValidCommand() with { Cpf = "123.456.789-09" };
        var result = await _handler.Handle(second, CancellationToken.None);

        var error = Assert.Single(result.Errors);
        Assert.Equal("customer.email_already_registered", error.Code);
        Assert.Equal(ErrorType.Conflict, error.Type);
        Assert.Single(_repository.Customers);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmailWithDifferentCasing_ShouldReturnConflict()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        var second = ValidCommand() with { Cpf = "123.456.789-09", Email = "  ANA@Example.COM " };
        var result = await _handler.Handle(second, CancellationToken.None);

        Assert.Equal("customer.email_already_registered", Assert.Single(result.Errors).Code);
    }

    [Fact]
    public async Task Handle_WithDuplicateCpfAndEmail_ShouldReturnBothConflicts()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.Equal(2, result.Errors.Count);
        Assert.All(result.Errors, e => Assert.Equal(ErrorType.Conflict, e.Type));
    }

    [Fact]
    public void Result_Value_OnFailure_ShouldThrow()
    {
        var result = Result<Guid>.Failure(Error.Validation("x", "erro"));

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }
}