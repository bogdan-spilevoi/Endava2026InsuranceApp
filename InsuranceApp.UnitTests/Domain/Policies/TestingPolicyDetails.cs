using InsuranceApp.Domain.Policies;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.Policies;

public sealed record TestingPolicyDetails
{
    public PolicyNumber PolicyNumber { get; init; } = new("P1");
    public Guid ClientId { get; init; } = TestIds.New();
    public Guid BuildingId { get; init; } = TestIds.New();
    public Guid BrokerId { get; init; } = TestIds.New();
    public DateRange Period { get; init; }
        = new(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 2));
    public Money BasePremium { get; init; } = new(10m, "RON");
    public Money PreliminaryFinalPremium { get; init; } = new(10m, "RON");

    public IEnumerable<Guid>? AppliedFeeIds { get; init; }
    public IEnumerable<Guid>? AppliedRiskIds { get; init; }

    public static implicit operator CreatePolicyDraftParams(TestingPolicyDetails d)
        => new()
        {
            PolicyNumber = d.PolicyNumber,
            ClientId = d.ClientId,
            BuildingId = d.BuildingId,
            BrokerId = d.BrokerId,
            Period = d.Period,
            BasePremium = d.BasePremium,
            PreliminaryFinalPremium = d.PreliminaryFinalPremium,
            AppliedFeeIds = d.AppliedFeeIds,
            AppliedRiskIds = d.AppliedRiskIds
        };
}
