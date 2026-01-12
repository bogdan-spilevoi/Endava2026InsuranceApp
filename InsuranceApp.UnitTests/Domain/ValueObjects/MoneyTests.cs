using FluentAssertions;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.ValueObjects;

public sealed class MoneyTests
{
    [Fact]
    public void Ctor_ShouldUppercaseCurrency_AndRoundTo2Decimals_AwayFromZero()
    {
        var money = new Money(10.005m, "ron");

        money.CurrencyCode.Should().Be("RON");
        money.Amount.Should().Be(10.01m);
        money.ToString().Should().Be("10.01 RON");
    }

    [Fact]
    public void Ctor_ShouldRoundDown_WhenThirdDecimalIs4()
    {
        var money = new Money(10.004m, "eur");

        money.Amount.Should().Be(10.00m);
        money.CurrencyCode.Should().Be("EUR");
        money.ToString().Should().Be("10.00 EUR");
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenAmountNegative()
    {
        Action act = () => _ = new Money(-0.01m, "USD");

        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_WhenCurrencyCodeNullOrWhitespace(string? bad)
    {
        Action act = () => _ = new Money(1m, bad!);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenCurrencyCodeExceeds3()
    {
        Action act = () => _ = new Money(1m, "USDX");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void WithAmount_ShouldReturnNewMoney_WithSameCurrency_AndRoundedAmount()
    {
        var original = new Money(1.111m, "usd");
        var updated = original.WithAmount(2.555m);

        original.CurrencyCode.Should().Be("USD");
        original.Amount.Should().Be(1.11m);

        updated.CurrencyCode.Should().Be("USD");
        updated.Amount.Should().Be(2.56m);
    }
}
