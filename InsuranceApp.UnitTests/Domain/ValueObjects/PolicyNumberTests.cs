using FluentAssertions;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.ValueObjects;

public sealed class PolicyNumberTests
{
    [Fact]
    public void Ctor_ShouldTrim_UppercaseInvariant_AndSetValue()
    {
        var pn = new PolicyNumber("  ab-123  ");

        pn.Value.Should().Be("AB-123");
        pn.ToString().Should().Be("AB-123");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_WhenNullOrWhitespace(string? bad)
    {
        Action act = () => _ = new PolicyNumber(bad!);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenExceeds40()
    {
        var tooLong = new string('a', 41);

        Action act = () => _ = new PolicyNumber(tooLong);

        act.Should().Throw<Exception>();
    }
}
