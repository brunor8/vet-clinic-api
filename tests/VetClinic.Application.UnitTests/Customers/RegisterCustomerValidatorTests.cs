using VetClinic.Application.Common.Results;
using VetClinic.Application.Customers.RegisterCustomer;
using VetClinic.Domain.Customers;

namespace VetClinic.Application.UnitTests.Customers;

public class RegisterCustomerValidatorTests
{
    private static RegisterCustomerCommand ValidCommand() =>
        new("Ana Silva", "529.982.247-25", "ana@example.com", "(11) 99999-9999");

    [Fact]
    public void Validate_WithValidCommand_ShouldReturnNoErrors()
    {
        var errors = RegisterCustomerValidator.Validate(ValidCommand());

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyName_ShouldReturnNameError(string? name)
    {
        var errors = RegisterCustomerValidator.Validate(ValidCommand() with { Name = name });

        var error = Assert.Single(errors);
        Assert.Equal("name", error.Code);
        Assert.Equal(ErrorType.Validation, error.Type);
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldReturnNameError()
    {
        var name = new string('a', Customer.NameMaxLength + 1);

        var errors = RegisterCustomerValidator.Validate(ValidCommand() with { Name = name });

        var error = Assert.Single(errors);
        Assert.Equal("name", error.Code);
    }

    [Fact]
    public void Validate_WithInvalidCpf_ShouldReturnCpfError()
    {
        var errors = RegisterCustomerValidator.Validate(ValidCommand() with { Cpf = "111.111.111-11" });

        var error = Assert.Single(errors);
        Assert.Equal("cpf", error.Code);
        Assert.Equal("CPF inválido.", error.Message);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldReturnEmailError()
    {
        var errors = RegisterCustomerValidator.Validate(ValidCommand() with { Email = "ana@" });

        var error = Assert.Single(errors);
        Assert.Equal("email", error.Code);
    }

    [Fact]
    public void Validate_WithInvalidPhone_ShouldReturnPhoneError()
    {
        var errors = RegisterCustomerValidator.Validate(ValidCommand() with { Phone = "123" });

        var error = Assert.Single(errors);
        Assert.Equal("phone", error.Code);
    }

    [Fact]
    public void Validate_WithAllFieldsInvalid_ShouldReturnAllErrorsAtOnce()
    {
        var command = new RegisterCustomerCommand("", "123", "x", "1");

        var errors = RegisterCustomerValidator.Validate(command);

        Assert.Equal(4, errors.Count);
        Assert.Equal(["name", "cpf", "email", "phone"], errors.Select(e => e.Code));
    }

    [Fact]
    public void Validate_WithNullFields_ShouldReturnRequiredErrors()
    {
        var errors = RegisterCustomerValidator.Validate(new RegisterCustomerCommand(null, null, null, null));

        Assert.Equal(4, errors.Count);
        Assert.All(errors, e => Assert.Equal(ErrorType.Validation, e.Type));
    }
}