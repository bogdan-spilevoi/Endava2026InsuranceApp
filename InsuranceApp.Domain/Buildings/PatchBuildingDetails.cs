using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Domain.Buildings;

public sealed record PatchBuildingParams(
    Address? Address = null,
    Guid? CityId = null,
    int? ConstructionYear = null,
    BuildingType? BuildingType = null,
    int? NumberOfFloors = null,
    decimal? SurfaceArea = null,
    decimal? InsuredValue = null,
    bool? FloodZone = null,
    bool? EarthquakeRiskZone = null
);