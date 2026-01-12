using FluentAssertions;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.ValueObjects;

public sealed class PhoneNumberTests
{
    [Fact]
    public void Ctor_ShouldTrim_AndSetValue()
    {
        var phone = new PhoneNumber("  +40 700 000 000  ");

        phone.Value.Should().Be("+40 700 000 000");
        phone.ToString().Should().Be("+40 700 000 000");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_WhenNullOrWhitespace(string? bad)
    {
        Action act = () => _ = new PhoneNumber(bad!);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenExceeds32()
    {
        var tooLong = new string('1', 33);

        Action act = () => _ = new PhoneNumber(tooLong);

        act.Should().Throw<Exception>();
    }
}
