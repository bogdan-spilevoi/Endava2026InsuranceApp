using System.Diagnostics.CodeAnalysis;
using InsuranceApp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.Configurations;

[SuppressMessage(
    "SonarLint",
    "S2325",
    Justification = "EF Core requires Configure to be an instance method to implement IEntityTypeConfiguration")]
public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
               .HasMaxLength(120)
               .IsRequired();

        builder.Property(x => x.CountyCode)
               .HasMaxLength(20)
               .IsRequired(false);

        builder.Property(x => x.CountyId)
               .IsRequired();

        builder.Ignore(x => x.UniqueKey);

        builder.HasIndex(x => new { x.CountyId, x.Name, x.CountyCode })
               .IsUnique();
    }
}
