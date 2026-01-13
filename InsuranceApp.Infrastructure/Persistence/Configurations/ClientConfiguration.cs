using System.Diagnostics.CodeAnalysis;
using InsuranceApp.Domain.Clients;
using InsuranceApp.Domain.ValueObjects;
using InsuranceApp.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.Configurations;

[SuppressMessage(
    "SonarLint",
    "S2325",
    Justification = "EF Core requires Configure to be an instance method to implement IEntityTypeConfiguration")]
public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Type).HasConversion<int>().IsRequired();

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.IdentificationNumber).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => x.IdentificationNumber).IsUnique();

        builder.Property(x => x.Email)
                .HasConversion(
                v => v.HasValue ? v.Value.Value : null,
                v => v == null ? null : new Email(v))
                .HasColumnName("Email")
                .HasMaxLength(254)
                .IsRequired(false);

        builder.Property(x => x.Phone)
                .HasConversion(
                v => v.HasValue ? v.Value.Value : null,
                v => v == null ? null : new PhoneNumber(v))
                .HasColumnName("Phone")
                .HasMaxLength(32)
                .IsRequired(false);

        builder.Property(x => x.PrimaryAddress)
                .HasConversion(new AddressConverter())
                .HasColumnName("PrimaryAddress")
                .HasMaxLength(500)
                .IsRequired(false);
    }
}