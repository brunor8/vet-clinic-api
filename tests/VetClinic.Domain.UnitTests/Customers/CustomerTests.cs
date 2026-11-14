using VetClinic.Domain.Common;
using VetClinic.Domain.Customers;

namespace VetClinic.Domain.UnitTests.Customers;

public class CustomerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 14, 12, 0, 0, TimeSpan.Zero);

    private static readonly Cpf ValidCpf = Cpf.Create("529.982.247-25");
    private static readonly Email ValidEmail = Email.Create("ana@example.com");
    private static readonly Phone ValidPhone = Phone.Create("(11) 99999-9999");

    private static Customer CreateValidCustomer() =>
        Customer.Register("Ana Silva", ValidCpf, ValidEmail, ValidPhone, Now);

    [Fact]
    public void Register_WithValidData_ShouldCreateActiveCustomer()
    {
        var customer = CreateValidCustomer();

        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("Ana Silva", customer.Name);
        Assert.Equal(ValidCpf, customer.Cpf);
        Assert.Equal(ValidEmail, customer.Email);
        Assert.Equal(ValidPhone, customer.Phone);
        Assert.True(customer.IsActive);
        Assert.Equal(Now, customer.CreatedAt);
    }

    [Fact]
    public void Register_ShouldTrimName()
    {
        var customer = Customer.Register("  Ana Silva  ", ValidCpf, ValidEmail, ValidPhone, Now);

        Assert.Equal("Ana Silva", customer.Name);
    }

    [Fact]
    public void Register_TwoCustomers_ShouldHaveDifferentIds()
    {
        Assert.NotEqual(CreateValidCustomer().Id, CreateValidCustomer().Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Register_WithEmptyName_ShouldThrow(string? name)
    {
        var ex = Assert.Throws<DomainException>(() =>
            Customer.Register(name, ValidCpf, ValidEmail, ValidPhone, Now));

        Assert.Equal("Nome é obrigatório.", ex.Message);
    }

    [Fact]
    public void Register_WithNameLongerThanMax_ShouldThrow()
    {
        var name = new string('a', Customer.NameMaxLength + 1);

        var ex = Assert.Throws<DomainException>(() =>
            Customer.Register(name, ValidCpf, ValidEmail, ValidPhone, Now));

        Assert.Equal($"Nome deve ter no máximo {Customer.NameMaxLength} caracteres.", ex.Message);
    }

    [Fact]
    public void Register_WithNameOfExactlyMaxLength_ShouldSucceed()
    {
        var name = new string('a', Customer.NameMaxLength);

        var customer = Customer.Register(name, ValidCpf, ValidEmail, ValidPhone, Now);

        Assert.Equal(Customer.NameMaxLength, customer.Name.Length);
    }

    [Fact]
    public void UpdateContact_WithValidData_ShouldChangeContactButKeepCpf()
    {
        var customer = CreateValidCustomer();
        var newEmail = Email.Create("novo@example.com");
        var newPhone = Phone.Create("(21) 98888-7777");

        customer.UpdateContact("Ana Souza", newEmail, newPhone);

        Assert.Equal("Ana Souza", customer.Name);
        Assert.Equal(newEmail, customer.Email);
        Assert.Equal(newPhone, customer.Phone);
        Assert.Equal(ValidCpf, customer.Cpf);
    }

    [Fact]
    public void UpdateContact_WithEmptyName_ShouldThrowAndKeepPreviousState()
    {
        var customer = CreateValidCustomer();

        Assert.Throws<DomainException>(() =>
            customer.UpdateContact("", Email.Create("novo@example.com"), ValidPhone));

        Assert.Equal("Ana Silva", customer.Name);
        Assert.Equal(ValidEmail, customer.Email);
    }

    [Fact]
    public void Deactivate_ShouldMarkCustomerAsInactive()
    {
        var customer = CreateValidCustomer();

        customer.Deactivate();

        Assert.False(customer.IsActive);
    }
}