using FluentAssertions;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.ValueObjects;

public sealed class AddressTests
{
    [Fact]
    public void Ctor_ShouldTrimStreetAndNumber_AndNormalizeAdditionalToNull_WhenWhitespace()
    {
        var address = new Address("  Main St  ", "  10  ", "   ");

        address.Street.Should().Be("Main St");
        address.Number.Should().Be("10");
        address.Additional.Should().BeNull();

        address.ToString().Should().Be("Main St, 10");
    }

    [Fact]
    public void Ctor_ShouldTrimAdditional_WhenProvided()
    {
        var address = new Address("Main St", "10", "  Apt 2  ");

        address.Additional.Should().Be("Apt 2");
        address.ToString().Should().Be("Main St, 10, Apt 2");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_WhenStreetIsNullOrWhitespace(string? badStreet)
    {
        Action act = () => _ = new Address(badStreet!, "10");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenStreetExceeds200()
    {
        var tooLong = new string('s', 201);

        Action act = () => _ = new Address(tooLong, "10");

        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_WhenNumberIsNullOrWhitespace(string? badNumber)
    {
        Action act = () => _ = new Address("Main", badNumber!);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenNumberExceeds30()
    {
        var tooLong = new string('1', 31);

        Action act = () => _ = new Address("Main", tooLong);

        act.Should().Throw<Exception>();
    }
}
