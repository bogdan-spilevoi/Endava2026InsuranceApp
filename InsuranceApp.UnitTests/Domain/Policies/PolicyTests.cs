using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Policies;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.Policies;

public sealed class PolicyTests
{
    [Fact]
    public void CreateDraft_ShouldCreateDraft_WithAppliedIdsDistinct()
    {
        var f1 = TestIds.New();
        var f2 = TestIds.New();
        var r1 = TestIds.New();

        var p = DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            AppliedFeeIds = new[] { f1, f1, f2 },
            AppliedRiskIds = new[] { r1, r1 }
        });

        p.Status.Should().Be(PolicyStatus.Draft);
        p.AppliedFeeConfigurationIds.Should().BeEquivalentTo(new[] { f1, f2 });
        p.AppliedRiskFactorConfigurationIds.Should().BeEquivalentTo(new[] { r1 });
    }

    [Fact]
    public void CreateDraft_ShouldThrow_WhenClientOrBuildingOrBrokerEmpty()
    {
        var act1 = () => DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            ClientId = Guid.Empty
        });
        act1.Should().Throw<DomainException>();

        var act2 = () => DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            BuildingId = Guid.Empty
        });
        act2.Should().Throw<DomainException>();

        var act3 = () => DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            BrokerId = Guid.Empty
        });
        act3.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateDraft_ShouldThrow_WhenBasePremiumNotPositive()
    {
        var act = () => DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            BasePremium = new Money(0, "RON"),
            PreliminaryFinalPremium = new Money(1, "RON")
        });

        act.Should().Throw<DomainException>().WithMessage("*Base premium*");
    }

    [Fact]
    public void CreateDraft_ShouldThrow_WhenCurrencyMismatch()
    {
        var act = () => DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            BasePremium = new Money(10, "RON"),
            PreliminaryFinalPremium = new Money(11, "EUR")
        });

        act.Should().Throw<DomainException>().WithMessage("*same currency*");
    }

    [Fact]
    public void RecalculatePremium_ShouldUpdateFinalPremium_AndAppliedIdsDistinct()
    {
        var p = DomainFactory.DraftPolicy();
        var f1 = TestIds.New();
        var f2 = TestIds.New();
        var r1 = TestIds.New();

        p.RecalculatePremium(
            new Money(123.45m, "RON"),
            feeIds: new[] { f1, f1, f2 },
            riskIds: new[] { r1, r1 });

        p.FinalPremium.Amount.Should().Be(123.45m);
        p.AppliedFeeConfigurationIds.Should().BeEquivalentTo(new[] { f1, f2 });
        p.AppliedRiskFactorConfigurationIds.Should().BeEquivalentTo(new[] { r1 });
    }

    [Fact]
    public void RecalculatePremium_ShouldThrow_WhenCurrencyMismatchOrNegative()
    {
        var p = DomainFactory.DraftPolicy();

        var act1 = () => p.RecalculatePremium(new Money(1m, "EUR"), Array.Empty<Guid>(), Array.Empty<Guid>());
        act1.Should().Throw<DomainException>().WithMessage("*Currency mismatch*");

        var act2 = () => p.RecalculatePremium(new Money(-1m, "RON"), Array.Empty<Guid>(), Array.Empty<Guid>());
        act2.Should().Throw<DomainException>().WithMessage("Amount must be >= 0.");
    }

    [Fact]
    public void Activate_ShouldTransitionDraftToActive_WhenValid()
    {
        var p = DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            Period = new DateRange(new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 2))
        });

        p.Activate(today: new DateOnly(2026, 1, 12), forbidPastStartDate: true);

        p.Status.Should().Be(PolicyStatus.Active);
        p.ActivatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Activate_ShouldThrow_WhenNotDraft()
    {
        var p = DomainFactory.DraftPolicy();
        p.Activate(new DateOnly(2026, 1, 12), false);

        var act = () => p.Activate(new DateOnly(2026, 1, 12), false);
        act.Should().Throw<DomainException>().WithMessage("*Only Draft*");
    }

    [Fact]
    public void Activate_ShouldThrow_WhenStartDateInPast_IfForbidden()
    {
        var p = DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            Period = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 2))
        });

        var act = () => p.Activate(today: new DateOnly(2026, 1, 12), forbidPastStartDate: true);

        act.Should().Throw<DomainException>().WithMessage("*start date*past*");
    }

    [Fact]
    public void Activate_ShouldAllowPastStartDate_WhenRuleDisabled()
    {
        var p = DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            Period = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 2))
        });

        p.Activate(today: new DateOnly(2026, 1, 12), forbidPastStartDate: false);

        p.Status.Should().Be(PolicyStatus.Active);
    }

    [Fact]
    public void Cancel_ShouldOnlyWorkFromActive_AndRequireReason()
    {
        var p = DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            Period = new DateRange(new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 2))
        });

        p.Activate(today: new DateOnly(2026, 1, 12), forbidPastStartDate: true);

        var emptyReason = () => p.Cancel(new DateOnly(2026, 3, 1), "");
        emptyReason.Should().Throw<DomainException>();

        p.Cancel(new DateOnly(2026, 3, 1), "Client request");
        p.Status.Should().Be(PolicyStatus.Cancelled);
        p.CancelledAtUtc.Should().NotBeNull();
        p.CancellationEffectiveDate.Should().Be(new DateOnly(2026, 3, 1));
        p.CancellationReason.Should().Be("Client request");

        var recalc = () => p.RecalculatePremium(new Money(1, "RON"), Array.Empty<Guid>(), Array.Empty<Guid>());
        recalc.Should().Throw<DomainException>().WithMessage("*Cancelled policy*");
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenNotActive()
    {
        var p = DomainFactory.DraftPolicy();
        var act = () => p.Cancel(new DateOnly(2026, 3, 1), "Reason");
        act.Should().Throw<DomainException>().WithMessage("*Only Active*");
    }

    [Fact]
    public void MarkExpired_ShouldSetExpired_WhenEndBeforeToday_AndNotCancelled()
    {
        var p = DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            Period = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5))
        });

        p.MarkExpired(today: new DateOnly(2026, 1, 12));
        p.Status.Should().Be(PolicyStatus.Expired);

        var recalc = () => p.RecalculatePremium(new Money(1, "RON"), Array.Empty<Guid>(), Array.Empty<Guid>());
        recalc.Should().Throw<DomainException>().WithMessage("*Expired policy*");
    }

    [Fact]
    public void MarkExpired_ShouldDoNothing_WhenNotPastEnd()
    {
        var p = DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            Period = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 2, 1))
        });

        p.MarkExpired(today: new DateOnly(2026, 1, 12));
        p.Status.Should().Be(PolicyStatus.Draft);
    }

    [Fact]
    public void MarkExpired_ShouldDoNothing_WhenCancelled()
    {
        var p = DomainFactory.DraftPolicy(new TestingPolicyDetails
        {
            Period = new DateRange(new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 2))
        });

        p.Activate(new DateOnly(2026, 1, 12), true);
        p.Cancel(new DateOnly(2026, 2, 15), "Reason");

        p.MarkExpired(today: new DateOnly(2027, 1, 1));
        p.Status.Should().Be(PolicyStatus.Cancelled);
    }
}
