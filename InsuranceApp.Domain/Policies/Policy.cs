using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Domain.Policies;

public sealed class Policy : AggregateRoot<Guid>
{
    public PolicyNumber PolicyNumber { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid BuildingId { get; private set; }
    public Guid BrokerId { get; private set; }

    public PolicyStatus Status { get; private set; }
    public DateRange Period { get; private set; }

    public Money BasePremium { get; private set; }
    public Money FinalPremium { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public DateTime? ActivatedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public DateOnly? CancellationEffectiveDate { get; private set; }
    public string? CancellationReason { get; private set; }

    private readonly List<Guid> _appliedFeeConfigurationIds = new();
    private readonly List<Guid> _appliedRiskFactorConfigurationIds = new();
    public IReadOnlyCollection<Guid> AppliedFeeConfigurationIds => _appliedFeeConfigurationIds.AsReadOnly();
    public IReadOnlyCollection<Guid> AppliedRiskFactorConfigurationIds => _appliedRiskFactorConfigurationIds.AsReadOnly();

    private Policy(
        Guid id,
        PolicyNumber policyNumber,
        Guid clientId,
        Guid buildingId,
        Guid brokerId,
        DateRange period,
        Money basePremium,
        Money finalPremium) : base(id)
    {
        if (clientId == Guid.Empty) throw new DomainException("ClientId is required.");
        if (buildingId == Guid.Empty) throw new DomainException("BuildingId is required.");
        if (brokerId == Guid.Empty) throw new DomainException("BrokerId is required.");

        PolicyNumber = policyNumber;
        ClientId = clientId;
        BuildingId = buildingId;
        BrokerId = brokerId;

        Period = period;

        if (basePremium.Amount <= 0) throw new DomainException("Base premium must be > 0.");
        if (finalPremium.CurrencyCode != basePremium.CurrencyCode) throw new DomainException("Final premium must use same currency as base premium.");
        if (finalPremium.Amount < 0) throw new DomainException("Final premium cannot be negative.");

        BasePremium = basePremium;
        FinalPremium = finalPremium;

        Status = PolicyStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public static Policy CreateDraft(Guid id, CreatePolicyDraftParams p)
    {
        Guard.NotNull(p, nameof(p));

        var policy = new Policy(
            id,
            p.PolicyNumber,
            p.ClientId,
            p.BuildingId,
            p.BrokerId,
            p.Period,
            p.BasePremium,
            p.PreliminaryFinalPremium
        );

        if (p.AppliedFeeIds is not null)
            policy._appliedFeeConfigurationIds.AddRange(p.AppliedFeeIds.Distinct());

        if (p.AppliedRiskIds is not null)
            policy._appliedRiskFactorConfigurationIds.AddRange(p.AppliedRiskIds.Distinct());

        return policy;
    }


    public void RecalculatePremium(Money newFinalPremium, IEnumerable<Guid> feeIds, IEnumerable<Guid> riskIds)
    {
        EnsureNotCancelledOrExpired();
        if (newFinalPremium.CurrencyCode != BasePremium.CurrencyCode)
            throw new DomainException("Currency mismatch for final premium recalculation.");

        if (newFinalPremium.Amount < 0) throw new DomainException("Final premium cannot be negative.");

        FinalPremium = newFinalPremium;
        _appliedFeeConfigurationIds.Clear();
        _appliedFeeConfigurationIds.AddRange(feeIds.Distinct());
        _appliedRiskFactorConfigurationIds.Clear();
        _appliedRiskFactorConfigurationIds.AddRange(riskIds.Distinct());

        Touch();
    }

    public void Activate(DateOnly today, bool forbidPastStartDate = true)
    {
        if (Status != PolicyStatus.Draft)
            throw new DomainException("Only Draft policies can be activated.");

        if (forbidPastStartDate && Period.Start < today)
            throw new DomainException("Policy start date cannot be in the past.");

        if (BasePremium.Amount <= 0) throw new DomainException("Base premium is required.");
        if (FinalPremium.Amount < 0) throw new DomainException("Final premium is invalid.");

        Status = PolicyStatus.Active;
        ActivatedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Cancel(DateOnly effectiveDate, string reason)
    {
        if (Status != PolicyStatus.Active)
            throw new DomainException("Only Active policies can be cancelled.");

        Guard.NotNullOrWhiteSpace(reason, nameof(reason), 500);

        Status = PolicyStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
        CancellationEffectiveDate = effectiveDate;
        CancellationReason = reason.Trim();
        Touch();
    }

    public void MarkExpired(DateOnly today)
    {
        if (Status == PolicyStatus.Cancelled) return;
        if (Status == PolicyStatus.Expired) return;

        if (Period.End < today)
        {
            Status = PolicyStatus.Expired;
            Touch();
        }
    }

    private void EnsureNotCancelledOrExpired()
    {
        if (Status == PolicyStatus.Cancelled) throw new DomainException("Cancelled policy cannot be modified.");
        if (Status == PolicyStatus.Expired) throw new DomainException("Expired policy cannot be modified.");
    }

    private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}