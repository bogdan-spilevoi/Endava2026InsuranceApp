using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Domain.Buildings;

public sealed record UpdateBuildingParams(
    Address Address,
    Guid CityId,
    int ConstructionYear,
    BuildingType BuildingType,
    int NumberOfFloors,
    decimal SurfaceArea,
    decimal InsuredValue,
    bool FloodZone,
    bool EarthquakeRiskZone
);