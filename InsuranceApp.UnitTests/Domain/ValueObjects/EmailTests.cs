using FluentAssertions;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Ctor_ShouldTrim_AndSetValue_WhenValid()
    {
        var email = new Email("  john@doe.com  ");

        email.Value.Should().Be("john@doe.com");
        email.ToString().Should().Be("john@doe.com");
    }

    [Theory]
    [InlineData("john.doe.com")]
    [InlineData("@john.doe.com")]
    [InlineData("john@")]
    public void Ctor_ShouldThrowDomainException_WhenFormatInvalid(string bad)
    {
        Action act = () => _ = new Email(bad);

        act.Should().Throw<Exception>()
            .WithMessage("Invalid email format.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_WhenNullOrWhitespace(string? bad)
    {
        Action act = () => _ = new Email(bad!);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenExceeds254()
    {
        var longLocal = new string('a', 250);
        var tooLong = longLocal + "@b.com";

        Action act = () => _ = new Email(tooLong);

        act.Should().Throw<Exception>();
    }
}
