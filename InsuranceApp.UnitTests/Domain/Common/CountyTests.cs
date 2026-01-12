using FluentAssertions;
using InsuranceApp.Domain.Common;

namespace InsuranceApp.UnitTests.Domain.Common;

public sealed class CountyTests
{
    [Fact]
    public void Ctor_ShouldTrimName_AndSetCountryId_AndInitCities()
    {
        var id = Guid.NewGuid();
        var countryId = Guid.NewGuid();

        var county = new County(id, "  Ilfov  ", countryId);

        county.Id.Should().Be(id);
        county.CountryId.Should().Be(countryId);
        county.Name.Should().Be("Ilfov");
        county.Cities.Should().NotBeNull();
        county.Cities.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_ForNullOrWhitespaceName(string? badName)
    {
        Action act = () => _ = new County(Guid.NewGuid(), badName!, Guid.NewGuid());

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_ForNameLongerThan120()
    {
        var tooLong = new string('a', 121);

        Action act = () => _ = new County(Guid.NewGuid(), tooLong, Guid.NewGuid());

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void AddCity_ShouldCreateCity_AddToCollection_AndReturnSameInstance()
    {
        var countyId = Guid.NewGuid();
        var countryId = Guid.NewGuid();
        var county = new County(countyId, "Iasi", countryId);

        var cityId = Guid.NewGuid();
        var returned = county.AddCity(cityId, "  Pascani  ", "  is  ");

        returned.Should().NotBeNull();
        returned.Id.Should().Be(cityId);
        returned.CountyId.Should().Be(countyId);
        returned.Name.Should().Be("Pascani");
        returned.CountyCode.Should().Be("IS");

        county.Cities.Should().ContainSingle()
            .Which.Should().BeSameAs(returned);

        county.Cities.Single().UniqueKey.Should().Be("Pascani|IS");
    }

    [Fact]
    public void Cities_ShouldBeReadOnlyView_ButReflectAdds()
    {
        var county = new County(Guid.NewGuid(), "Cluj", Guid.NewGuid());
        county.Cities.Should().BeEmpty();

        county.AddCity(Guid.NewGuid(), "Cluj-Napoca", "cj");

        county.Cities.Should().ContainSingle();
    }
}
