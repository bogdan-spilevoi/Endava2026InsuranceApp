using FluentAssertions;
using InsuranceApp.Domain.Common;

namespace InsuranceApp.UnitTests.Domain.Common;

public sealed class CityTests
{
    [Fact]
    public void Ctor_ShouldTrimName_AndSetIds()
    {
        var id = Guid.NewGuid();
        var countyId = Guid.NewGuid();

        var city = new City(id, "  Bucharest  ", countyId);

        city.Id.Should().Be(id);
        city.CountyId.Should().Be(countyId);
        city.Name.Should().Be("Bucharest");
        city.CountyCode.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_ForNullOrWhitespaceName(string? badName)
    {
        Action act = () => _ = new City(Guid.NewGuid(), badName!, Guid.NewGuid());

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_ForNameLongerThan120()
    {
        var tooLong = new string('a', 121);

        Action act = () => _ = new City(Guid.NewGuid(), tooLong, Guid.NewGuid());

        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldSetCountyCodeNull_WhenNullOrWhitespace(string? code)
    {
        var city = new City(Guid.NewGuid(), "Iasi", Guid.NewGuid(), code);

        city.CountyCode.Should().BeNull();
    }

    [Fact]
    public void Ctor_ShouldTrimAndUppercaseCountyCode_WhenProvided()
    {
        var city = new City(Guid.NewGuid(), "Iasi", Guid.NewGuid(), "  ab-12  ");

        city.CountyCode.Should().Be("AB-12");
    }

    [Fact]
    public void UniqueKey_ShouldBeName_WhenCountyCodeIsNull()
    {
        var city = new City(Guid.NewGuid(), "Cluj", Guid.NewGuid(), null);

        city.UniqueKey.Should().Be("Cluj");
    }

    [Fact]
    public void UniqueKey_ShouldBeNamePipeCountyCode_WhenCountyCodeIsNotNull()
    {
        var city = new City(Guid.NewGuid(), "Cluj", Guid.NewGuid(), "cj");

        city.UniqueKey.Should().Be("Cluj|CJ");
    }
}
