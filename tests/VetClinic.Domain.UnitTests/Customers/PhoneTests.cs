using VetClinic.Domain.Common;
using VetClinic.Domain.Customers;

namespace VetClinic.Domain.UnitTests.Customers;

public class PhoneTests
{
    [Theory]
    [InlineData("(11) 99999-9999", "11999999999")]
    [InlineData("11 3333-4444", "1133334444")]
    [InlineData("+55 (11) 99999-9999", "5511999999999")]
    [InlineData("5511999999999", "5511999999999")]
    public void Create_WithValidPhone_ShouldKeepOnlyDigits(string input, string expected)
    {
        var phone = Phone.Create(input);

        Assert.Equal(expected, phone.Value);
    }

    [Fact]
    public void Create_WithAndWithoutMask_ShouldBeEqual()
    {
        Assert.Equal(Phone.Create("(11) 99999-9999"), Phone.Create("11999999999"));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("999999999")]        // 9 digits
    [InlineData("55119999999999")]   // 14 digits
    [InlineData("telefone")]
    public void Create_WithInvalidLength_ShouldThrow(string input)
    {
        var ex = Assert.Throws<DomainException>(() => Phone.Create(input));

        Assert.Equal("Telefone inválido.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowRequired(string? input)
    {
        var ex = Assert.Throws<DomainException>(() => Phone.Create(input));

        Assert.Equal("Telefone é obrigatório.", ex.Message);
    }
}