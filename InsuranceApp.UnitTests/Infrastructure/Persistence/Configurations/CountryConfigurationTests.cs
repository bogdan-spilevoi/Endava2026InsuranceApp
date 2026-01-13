using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.UnitTests.Infrastructure.Persistence.Configurations;

public sealed class CountryConfigurationTests
{
    [Fact]
    public void Configure_maps_country_correctly()
    {
        var model = EfModelTestHelper.BuildModel<Country>(b =>
            new CountryConfiguration().Configure(b));

        var entity = model.FindEntityType(typeof(Country));
        entity.Should().NotBeNull();
        entity!.GetTableName().Should().Be("Countries");

        var name = entity.FindProperty(nameof(Country.Name));
        name.Should().NotBeNull();
        name!.IsNullable.Should().BeFalse();
        name.GetMaxLength().Should().Be(120);

        entity.GetIndexes().Should().Contain(ix =>
            ix.IsUnique &&
            ix.Properties.Count == 1 &&
            ix.Properties[0].Name == nameof(Country.Name));
    }
}
