using FluentAssertions;
using InsuranceApp.Domain.Buildings;
using InsuranceApp.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InsuranceApp.UnitTests.Infrastructure.Persistence.Configurations;

public sealed class BuildingConfigurationTests
{
    [Fact]
    public void Configure_maps_building_correctly()
    {
        var model = EfModelTestHelper.BuildModel<Building>(b =>
            new BuildingConfiguration().Configure(b));

        var entity = model.FindEntityType(typeof(Building));
        entity.Should().NotBeNull();
        entity!.GetTableName().Should().Be("Buildings");

        var pk = entity.FindPrimaryKey();
        pk.Should().NotBeNull();
        pk!.Properties.Should().ContainSingle();
        pk.Properties[0].Name.Should().Be(nameof(Building.Id));

        entity.FindProperty(nameof(Building.OwnerClientId))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Building.CityId))!.IsNullable.Should().BeFalse();

        entity.GetIndexes().Should().Contain(ix =>
            ix.Properties.Count == 1 &&
            ix.Properties[0].Name == nameof(Building.OwnerClientId));

        entity.GetIndexes().Should().Contain(ix =>
            ix.Properties.Count == 1 &&
            ix.Properties[0].Name == nameof(Building.CityId));

        entity.FindProperty(nameof(Building.BuildingType))!
              .Should().NotBeNull();

        var surface = entity.FindProperty(nameof(Building.SurfaceArea))!;
        surface.GetPrecision().Should().Be(18);
        surface.GetScale().Should().Be(2);

        var insured = entity.FindProperty(nameof(Building.InsuredValue))!;
        insured.GetPrecision().Should().Be(18);
        insured.GetScale().Should().Be(2);

        var address = entity.FindProperty(nameof(Building.Address))!;
        address.IsNullable.Should().BeFalse();
        address.GetColumnName(StoreObjectIdentifier.Table("Buildings", null)).Should().Be("Address");
        address.GetMaxLength().Should().Be(500);
        address.GetValueConverter().Should().NotBeNull();
    }
}
