using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.UnitTests.Infrastructure.Persistence.Configurations;

public sealed class CountyConfigurationTests
{
    [Fact]
    public void Configure_maps_county_correctly()
    {
        var model = EfModelTestHelper.BuildModel<County>(b =>
            new CountyConfiguration().Configure(b));

        var entity = model.FindEntityType(typeof(County));
        entity.Should().NotBeNull();
        entity!.GetTableName().Should().Be("Counties");

        var name = entity.FindProperty(nameof(County.Name));
        name.Should().NotBeNull();
        name!.IsNullable.Should().BeFalse();
        name.GetMaxLength().Should().Be(120);

        var countryId = entity.FindProperty(nameof(County.CountryId));
        countryId.Should().NotBeNull();
        countryId!.IsNullable.Should().BeFalse();

        entity.GetIndexes().Should().Contain(ix =>
            ix.IsUnique &&
            ix.Properties.Count == 2 &&
            ix.Properties[0].Name == nameof(County.CountryId) &&
            ix.Properties[1].Name == nameof(County.Name));
    }
}
