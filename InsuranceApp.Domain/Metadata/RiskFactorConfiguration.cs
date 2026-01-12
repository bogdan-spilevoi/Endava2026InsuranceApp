using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Metadata;

public sealed class RiskFactorConfiguration : AggregateRoot<Guid>
{
    public RiskFactorLevel Level { get; private set; }
    public Guid? ReferenceId { get; private set; } // CountyId/CityId/CountryId; null for BuildingType-level
    public BuildingType? BuildingType { get; private set; } // used when Level == BuildingType
    public decimal AdjustmentPercentage { get; private set; } // can be +/- (e.g. 0.05m or -0.02m)
    public bool IsActive { get; private set; }

    public RiskFactorConfiguration(
        Guid id,
        RiskFactorLevel level,
        Guid? referenceId,
        BuildingType? buildingType,
        decimal adjustmentPercentage,
        bool isActive) : base(id)
    {
        if (adjustmentPercentage <= -1m) throw new DomainException("AdjustmentPercentage must be > -1.00.");
        Level = level;

        switch (level)
        {
            case RiskFactorLevel.BuildingType:
                if (buildingType is null) throw new DomainException("BuildingType is required when Level is BuildingType.");
                ReferenceId = null;
                BuildingType = buildingType;
                break;

            case RiskFactorLevel.Country:
            case RiskFactorLevel.County:
            case RiskFactorLevel.City:
                if (referenceId is null) throw new DomainException("ReferenceId is required for geography-based risk factors.");
                ReferenceId = referenceId;
                BuildingType = null;
                break;

            default:
                throw new DomainException("Unsupported RiskFactor level.");
        }

        AdjustmentPercentage = adjustmentPercentage;
        IsActive = isActive;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void UpdateAdjustment(decimal adjustmentPercentage)
    {
        if (adjustmentPercentage <= -1m) throw new DomainException("AdjustmentPercentage must be > -1.00.");
        AdjustmentPercentage = adjustmentPercentage;
    }
}