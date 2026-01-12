using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Metadata;

namespace InsuranceApp.UnitTests.Domain.Metadata;

public sealed class FeeConfigurationTests
{
    [Fact]
    public void Ctor_ShouldCreate_AndIsEffectiveOnWorks()
    {
        var from = new DateOnly(2026, 1, 1);
        var to = new DateOnly(2026, 12, 31);
        var fee = DomainFactory.Fee(percentage: 0.10m, from: from, to: to, active: true);

        fee.IsEffectiveOn(new DateOnly(2026, 6, 1)).Should().BeTrue();
        fee.IsEffectiveOn(new DateOnly(2025, 12, 31)).Should().BeFalse();
        fee.IsEffectiveOn(new DateOnly(2027, 1, 1)).Should().BeFalse();
    }

    [Fact]
    public void Ctor_ShouldAllowOpenEndedEffectiveTo()
    {
        var from = new DateOnly(2026, 1, 1);
        var fee = DomainFactory.Fee(percentage: 0.05m, from: from, to: null, active: true);

        fee.IsEffectiveOn(new DateOnly(2030, 1, 1)).Should().BeTrue();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenNameInvalidOrEffectiveToBeforeFrom()
    {
        var from = new DateOnly(2026, 1, 1);
        var to = new DateOnly(2025, 1, 1);

        var act1 = () => new FeeConfiguration(TestIds.New(), "", FeeConfigurationType.AdminFee, 0.1m, from, null, true);
        act1.Should().Throw<DomainException>();

        var act2 = () => new FeeConfiguration(TestIds.New(), "Fee", FeeConfigurationType.AdminFee, 0.1m, from, to, true);
        act2.Should().Throw<DomainException>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenPercentageTooLow()
    {
        var from = new DateOnly(2026, 1, 1);
        var act = () => DomainFactory.Fee(percentage: -1.0m, from: from, to: null, active: true);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdatePercentage_ShouldUpdate_AndValidate()
    {
        var fee = DomainFactory.Fee(percentage: 0.1m, from: new DateOnly(2026, 1, 1), to: null, active: true);

        fee.UpdatePercentage(0.2m);
        fee.Percentage.Should().Be(0.2m);

        var act = () => fee.UpdatePercentage(-1m);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ActivateDeactivate_ShouldToggle_AndAffectIsEffectiveOn()
    {
        var fee = DomainFactory.Fee(percentage: 0.1m, from: new DateOnly(2026, 1, 1), to: null, active: true);
        fee.IsEffectiveOn(new DateOnly(2026, 2, 1)).Should().BeTrue();

        fee.Deactivate();
        fee.IsEffectiveOn(new DateOnly(2026, 2, 1)).Should().BeFalse();

        fee.Activate();
        fee.IsEffectiveOn(new DateOnly(2026, 2, 1)).Should().BeTrue();
    }
}