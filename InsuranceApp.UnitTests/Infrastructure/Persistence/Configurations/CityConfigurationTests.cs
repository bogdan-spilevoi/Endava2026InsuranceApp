using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.UnitTests.Infrastructure.Persistence.Configurations;

public sealed class CityConfigurationTests
{
    [Fact]
    public void Configure_maps_city_correctly()
    {
        var model = EfModelTestHelper.BuildModel<City>(b =>
            new CityConfiguration().Configure(b));

        var entity = model.FindEntityType(typeof(City));
        entity.Should().NotBeNull();
        entity!.GetTableName().Should().Be("Cities");

        var name = entity.FindProperty(nameof(City.Name));
        name.Should().NotBeNull();
        name!.IsNullable.Should().BeFalse();
        name.GetMaxLength().Should().Be(120);

        var countyId = entity.FindProperty(nameof(City.CountyId));
        countyId.Should().NotBeNull();
        countyId!.IsNullable.Should().BeFalse();

        var countyCode = entity.FindProperty(nameof(City.CountyCode));
        countyCode.Should().NotBeNull();
        countyCode!.IsNullable.Should().BeTrue();
        countyCode.GetMaxLength().Should().Be(20);

        // ignored computed property
        entity.FindProperty(nameof(City.UniqueKey)).Should().BeNull();

        entity.GetIndexes().Should().Contain(ix =>
            ix.IsUnique &&
            ix.Properties.Count == 3 &&
            ix.Properties[0].Name == nameof(City.CountyId) &&
            ix.Properties[1].Name == nameof(City.Name) &&
            ix.Properties[2].Name == nameof(City.CountyCode));
    }
}
