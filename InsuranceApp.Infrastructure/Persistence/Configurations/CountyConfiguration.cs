using System.Diagnostics.CodeAnalysis;
using InsuranceApp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.Configurations;

[SuppressMessage(
    "SonarLint",
    "S2325",
    Justification = "EF Core requires Configure to be an instance method to implement IEntityTypeConfiguration")]
public sealed class CountyConfiguration : IEntityTypeConfiguration<County>
{
    public void Configure(EntityTypeBuilder<County> builder)
    {
        builder.ToTable("Counties");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
               .HasMaxLength(120)
               .IsRequired();

        builder.Property(x => x.CountryId)
               .IsRequired();

        builder.HasIndex(x => new { x.CountryId, x.Name })
               .IsUnique();

        builder.Metadata
               .FindNavigation(nameof(County.Cities))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<City>("_cities")
               .WithOne()
               .HasForeignKey(x => x.CountyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
