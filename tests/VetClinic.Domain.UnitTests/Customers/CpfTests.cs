using VetClinic.Domain.Common;
using VetClinic.Domain.Customers;

namespace VetClinic.Domain.UnitTests.Customers;

public class CpfTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    [InlineData("123.456.789-09")]
    [InlineData("  529.982.247-25  ")]
    public void Create_WithValidCpf_ShouldSucceed(string input)
    {
        var cpf = Cpf.Create(input);

        Assert.Equal(11, cpf.Value.Length);
        Assert.All(cpf.Value, c => Assert.True(char.IsDigit(c)));
    }

    [Fact]
    public void Create_WithMask_ShouldStoreOnlyDigits()
    {
        var cpf = Cpf.Create("529.982.247-25");

        Assert.Equal("52998224725", cpf.Value);
    }

    [Fact]
    public void Formatted_ShouldReturnMaskedCpf()
    {
        var cpf = Cpf.Create("52998224725");

        Assert.Equal("529.982.247-25", cpf.Formatted());
    }

    [Fact]
    public void Create_WithAndWithoutMask_ShouldBeEqual()
    {
        var withMask = Cpf.Create("529.982.247-25");
        var withoutMask = Cpf.Create("52998224725");

        Assert.Equal(withMask, withoutMask);
    }

    [Theory]
    [InlineData("529.982.247-24")] // 2nd wrong digit
    [InlineData("529.982.247-35")] // 1st wrong digit
    [InlineData("12345678900")]
    public void Create_WithWrongCheckDigits_ShouldThrow(string input)
    {
        var ex = Assert.Throws<DomainException>(() => Cpf.Create(input));

        Assert.Equal("CPF inválido.", ex.Message);
    }

    [Theory]
    [InlineData("000.000.000-00")]
    [InlineData("111.111.111-11")]
    [InlineData("99999999999")]
    public void Create_WithRepeatedDigits_ShouldThrow(string input)
    {
        var ex = Assert.Throws<DomainException>(() => Cpf.Create(input));

        Assert.Equal("CPF inválido.", ex.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("529982247250")] // 12 digits
    [InlineData("abc")]
    public void Create_WithWrongLength_ShouldThrow(string input)
    {
        var ex = Assert.Throws<DomainException>(() => Cpf.Create(input));

        Assert.Equal("CPF inválido.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowRequired(string? input)
    {
        var ex = Assert.Throws<DomainException>(() => Cpf.Create(input));

        Assert.Equal("CPF é obrigatório.", ex.Message);
    }
}