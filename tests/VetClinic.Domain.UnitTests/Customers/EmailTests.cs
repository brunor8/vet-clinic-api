using VetClinic.Domain.Common;
using VetClinic.Domain.Customers;

namespace VetClinic.Domain.UnitTests.Customers;

public class EmailTests
{
    [Theory]
    [InlineData("ana@example.com")]
    [InlineData("ana.silva+vet@clinica.com.br")]
    public void Create_WithValidEmail_ShouldSucceed(string input)
    {
        var email = Email.Create(input);

        Assert.Equal(input, email.Value);
    }

    [Fact]
    public void Create_ShouldTrimAndLowercase()
    {
        var email = Email.Create("  ANA@Example.COM ");

        Assert.Equal("ana@example.com", email.Value);
    }

    [Fact]
    public void Create_WithDifferentCasing_ShouldBeEqual()
    {
        Assert.Equal(Email.Create("Ana@Example.com"), Email.Create("ana@example.com"));
    }

    [Theory]
    [InlineData("ana")]
    [InlineData("ana@")]
    [InlineData("@example.com")]
    [InlineData("ana@@example.com")]
    [InlineData("Ana Silva <ana@example.com>")] // formato com nome de exibição
    public void Create_WithInvalidFormat_ShouldThrow(string input)
    {
        var ex = Assert.Throws<DomainException>(() => Email.Create(input));

        Assert.Equal("E-mail inválido.", ex.Message);
    }

    [Fact]
    public void Create_WithMoreThan254Characters_ShouldThrow()
    {
        var input = new string('a', 250) + "@example.com";

        var ex = Assert.Throws<DomainException>(() => Email.Create(input));

        Assert.Equal("E-mail inválido.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowRequired(string? input)
    {
        var ex = Assert.Throws<DomainException>(() => Email.Create(input));

        Assert.Equal("E-mail é obrigatório.", ex.Message);
    }
}