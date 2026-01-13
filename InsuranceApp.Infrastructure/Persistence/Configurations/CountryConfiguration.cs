using System.Diagnostics.CodeAnalysis;
using InsuranceApp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.Configurations;

[SuppressMessage(
    "SonarLint",
    "S2325",
    Justification = "EF Core requires Configure to be an instance method to implement IEntityTypeConfiguration")]
public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
               .HasMaxLength(120)
               .IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Metadata
               .FindNavigation(nameof(Country.Counties))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<County>("_counties")
               .WithOne()
               .HasForeignKey(x => x.CountryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
