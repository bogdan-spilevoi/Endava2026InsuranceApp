using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Domain.Policies;

public sealed record CreatePolicyDraftParams
{
    public required PolicyNumber PolicyNumber { get; init; }
    public required Guid ClientId { get; init; }
    public required Guid BuildingId { get; init; }
    public required Guid BrokerId { get; init; }
    public required DateRange Period { get; init; }
    public required Money BasePremium { get; init; }
    public required Money PreliminaryFinalPremium { get; init; }

    public IEnumerable<Guid>? AppliedFeeIds { get; init; }
    public IEnumerable<Guid>? AppliedRiskIds { get; init; }
}
