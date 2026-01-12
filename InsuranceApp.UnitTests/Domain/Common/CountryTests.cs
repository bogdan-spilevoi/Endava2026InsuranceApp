using FluentAssertions;
using InsuranceApp.Domain.Common;

namespace InsuranceApp.UnitTests.Domain.Common;

public sealed class CountryTests
{
    [Fact]
    public void Ctor_ShouldTrimName_AndInitializeCounties()
    {
        var id = Guid.NewGuid();

        var country = new Country(id, "  Romania  ");

        country.Id.Should().Be(id);
        country.Name.Should().Be("Romania");
        country.Counties.Should().NotBeNull();
        country.Counties.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_ForNullOrWhitespaceName(string? badName)
    {
        Action act = () => _ = new Country(Guid.NewGuid(), badName!);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_ForNameLongerThan120()
    {
        var tooLong = new string('a', 121);

        Action act = () => _ = new Country(Guid.NewGuid(), tooLong);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void AddCounty_ShouldCreateCounty_AddToCollection_AndReturnSameInstance()
    {
        var countryId = Guid.NewGuid();
        var country = new Country(countryId, "Romania");

        var countyId = Guid.NewGuid();
        var returned = country.AddCounty(countyId, "  Cluj  ");

        returned.Should().NotBeNull();
        returned.Id.Should().Be(countyId);
        returned.CountryId.Should().Be(countryId);
        returned.Name.Should().Be("Cluj");

        country.Counties.Should().ContainSingle()
            .Which.Should().BeSameAs(returned);
    }
}
