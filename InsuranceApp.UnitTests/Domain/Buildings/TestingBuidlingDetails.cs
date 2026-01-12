namespace InsuranceApp.UnitTests.Domain.Buildings;

using InsuranceApp.Domain.Buildings;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

public sealed record TestingBuildingDetails
{
    public Guid? OwnerClientId { get; init; }
    public Address? Address { get; init; }
    public Guid? CityId { get; init; }

    public int ConstructionYear { get; init; } = 2000;
    public BuildingType BuildingType { get; init; } = BuildingType.Residential;
    public int NumberOfFloors { get; init; } = 2;
    public decimal SurfaceArea { get; init; } = 120;
    public decimal InsuredValue { get; init; } = 100_000;
    public bool FloodZone { get; init; }
    public bool EarthquakeRiskZone { get; init; }


    public static implicit operator CreateBuildingParams(TestingBuildingDetails t)
        => new CreateBuildingParams(
            OwnerClientId: t.OwnerClientId ?? TestIds.New(),
            Address: t.Address ?? DomainFactory.Address(),
            CityId: t.CityId ?? TestIds.New(),
            ConstructionYear: t.ConstructionYear,
            BuildingType: t.BuildingType,
            NumberOfFloors: t.NumberOfFloors,
            SurfaceArea: t.SurfaceArea,
            InsuredValue: t.InsuredValue,
            FloodZone: t.FloodZone,
            EarthquakeRiskZone: t.EarthquakeRiskZone
        );


    public static implicit operator UpdateBuildingParams(TestingBuildingDetails t)
        => new UpdateBuildingParams(
            Address: t.Address ?? DomainFactory.Address(),
            CityId: t.CityId ?? TestIds.New(),
            ConstructionYear: t.ConstructionYear,
            BuildingType: t.BuildingType,
            NumberOfFloors: t.NumberOfFloors,
            SurfaceArea: t.SurfaceArea,
            InsuredValue: t.InsuredValue,
            FloodZone: t.FloodZone,
            EarthquakeRiskZone: t.EarthquakeRiskZone
        );


    public static implicit operator PatchBuildingParams(TestingBuildingDetails t)
        => new PatchBuildingParams(
            Address: t.Address,
            CityId: t.CityId,
            ConstructionYear: t.ConstructionYear,
            BuildingType: t.BuildingType,
            NumberOfFloors: t.NumberOfFloors,
            SurfaceArea: t.SurfaceArea,
            InsuredValue: t.InsuredValue,
            FloodZone: t.FloodZone,
            EarthquakeRiskZone: t.EarthquakeRiskZone
        );
}
