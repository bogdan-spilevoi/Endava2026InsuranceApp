using FluentAssertions;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.ValueObjects;

public sealed class DateRangeTests
{
    [Fact]
    public void Ctor_ShouldSetStartAndEnd_WhenEndEqualsStart()
    {
        var d = new DateOnly(2026, 1, 12);
        var range = new DateRange(d, d);

        range.Start.Should().Be(d);
        range.End.Should().Be(d);
        range.Contains(d).Should().BeTrue();
    }

    [Fact]
    public void Ctor_ShouldThrowDomainException_WhenEndBeforeStart()
    {
        var start = new DateOnly(2026, 1, 12);
        var end = new DateOnly(2026, 1, 11);

        Action act = () => _ = new DateRange(start, end);

        act.Should().Throw<Exception>()
            .WithMessage("End date must be >= start date.");
    }

    [Fact]
    public void Contains_ShouldBeTrue_ForStartAndEnd_AndFalseOutside()
    {
        var start = new DateOnly(2026, 1, 1);
        var end = new DateOnly(2026, 1, 31);
        var range = new DateRange(start, end);

        range.Contains(new DateOnly(2026, 1, 1)).Should().BeTrue();
        range.Contains(new DateOnly(2026, 1, 31)).Should().BeTrue();
        range.Contains(new DateOnly(2025, 12, 31)).Should().BeFalse();
        range.Contains(new DateOnly(2026, 2, 1)).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldUseExpectedFormat()
    {
        var range = new DateRange(new DateOnly(2026, 1, 2), new DateOnly(2026, 1, 5));

        range.ToString().Should().Be("2026-01-02 .. 2026-01-05");
    }
}
