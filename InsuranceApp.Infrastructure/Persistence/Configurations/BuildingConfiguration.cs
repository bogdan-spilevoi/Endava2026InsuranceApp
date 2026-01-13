using InsuranceApp.Domain.Buildings;
using InsuranceApp.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.Configurations;

public sealed class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.OwnerClientId)
               .IsRequired();

        builder.Property(x => x.CityId)
               .IsRequired();

        builder.HasIndex(x => x.OwnerClientId);
        builder.HasIndex(x => x.CityId);

        builder.Property(x => x.BuildingType)
               .HasConversion<int>()
               .IsRequired();

        builder.Property(x => x.ConstructionYear)
               .IsRequired();

        builder.Property(x => x.NumberOfFloors)
               .IsRequired();

        builder.Property(x => x.SurfaceArea)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(x => x.InsuredValue)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(x => x.FloodZone)
               .IsRequired();

        builder.Property(x => x.EarthquakeRiskZone)
               .IsRequired();


        builder.Property(x => x.Address)
               .HasConversion(new AddressConverter())
               .HasColumnName("Address")
               .HasMaxLength(500)
               .IsRequired();
    }
}
