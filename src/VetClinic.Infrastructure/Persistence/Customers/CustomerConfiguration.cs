using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Application.Customers;
using VetClinic.Domain.Customers;

namespace VetClinic.Infrastructure.Persistence.Customers;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers", "customers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(Customer.NameMaxLength)
            .IsRequired();

        builder.Property(c => c.Cpf)
            .HasColumnName("cpf")
            .HasMaxLength(Cpf.Length)
            .IsFixedLength()
            .IsRequired()
            .HasConversion(v => v.Value, v => Cpf.Create(v));

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(254)
            .IsRequired()
            .HasConversion(v => v.Value, v => Email.Create(v));

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(13)
            .IsRequired()
            .HasConversion(v => v.Value, v => Phone.Create(v));

        builder.Property(c => c.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(c => c.Cpf).IsUnique().HasDatabaseName(CustomerConstraints.Cpf);
        builder.HasIndex(c => c.Email).IsUnique().HasDatabaseName(CustomerConstraints.Email);
    }
}