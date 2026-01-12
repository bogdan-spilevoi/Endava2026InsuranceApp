using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Domain.Buildings;

public sealed class Building : AggregateRoot<Guid>
{
    private readonly List<Guid> _policyIds = new();

    public Guid OwnerClientId { get; private set; }
    public Address Address { get; private set; }
    public Guid CityId { get; private set; }
    public int ConstructionYear { get; private set; }
    public BuildingType BuildingType { get; private set; }
    public int NumberOfFloors { get; private set; }
    public decimal SurfaceArea { get; private set; }
    public decimal InsuredValue { get; private set; }
    public bool FloodZone { get; private set; }
    public bool EarthquakeRiskZone { get; private set; }

    public IReadOnlyCollection<Guid> PolicyIds => _policyIds.AsReadOnly();

    public Building(Guid id, CreateBuildingParams p) : base(id)
    {
        Guard.NotNull(p, nameof(p));

        if (p.OwnerClientId == Guid.Empty)
            throw new DomainException("OwnerClientId is required.");

        if (p.CityId == Guid.Empty)
            throw new DomainException("CityId is required.");

        var currentYear = DateTime.UtcNow.Year;
        Guard.InRange(p.ConstructionYear, nameof(p.ConstructionYear), 1800, currentYear);
        Guard.InRange(p.NumberOfFloors, nameof(p.NumberOfFloors), 0, 300);
        Guard.Positive(p.SurfaceArea, nameof(p.SurfaceArea));
        Guard.Positive(p.InsuredValue, nameof(p.InsuredValue));

        OwnerClientId = p.OwnerClientId;
        Address = p.Address;
        CityId = p.CityId;
        ConstructionYear = p.ConstructionYear;
        BuildingType = p.BuildingType;
        NumberOfFloors = p.NumberOfFloors;
        SurfaceArea = p.SurfaceArea;
        InsuredValue = p.InsuredValue;
        FloodZone = p.FloodZone;
        EarthquakeRiskZone = p.EarthquakeRiskZone;
    }

    public void UpdateDetails(UpdateBuildingParams cmd)
    {
        Guard.NotNull(cmd, nameof(cmd));

        if (cmd.CityId == Guid.Empty)
            throw new DomainException("CityId is required.");

        var currentYear = DateTime.UtcNow.Year;
        Guard.InRange(cmd.ConstructionYear, nameof(cmd.ConstructionYear), 1800, currentYear);
        Guard.InRange(cmd.NumberOfFloors, nameof(cmd.NumberOfFloors), 0, 300);
        Guard.Positive(cmd.SurfaceArea, nameof(cmd.SurfaceArea));
        Guard.Positive(cmd.InsuredValue, nameof(cmd.InsuredValue));

        Address = cmd.Address;
        CityId = cmd.CityId;
        ConstructionYear = cmd.ConstructionYear;
        BuildingType = cmd.BuildingType;
        NumberOfFloors = cmd.NumberOfFloors;
        SurfaceArea = cmd.SurfaceArea;
        InsuredValue = cmd.InsuredValue;
        FloodZone = cmd.FloodZone;
        EarthquakeRiskZone = cmd.EarthquakeRiskZone;
    }

    public void PatchDetails(PatchBuildingParams patch)
    {
        Guard.NotNull(patch, nameof(patch));

        if (patch.CityId.HasValue)
        {
            if (patch.CityId.Value == Guid.Empty)
                throw new DomainException("CityId is required.");
            CityId = patch.CityId.Value;
        }

        if (patch.ConstructionYear.HasValue)
        {
            var currentYear = DateTime.UtcNow.Year;
            Guard.InRange(patch.ConstructionYear.Value, nameof(patch.ConstructionYear), 1800, currentYear);
            ConstructionYear = patch.ConstructionYear.Value;
        }

        if (patch.BuildingType.HasValue)
            BuildingType = patch.BuildingType.Value;

        if (patch.NumberOfFloors.HasValue)
        {
            Guard.InRange(patch.NumberOfFloors.Value, nameof(patch.NumberOfFloors), 0, 300);
            NumberOfFloors = patch.NumberOfFloors.Value;
        }

        if (patch.SurfaceArea.HasValue)
        {
            Guard.Positive(patch.SurfaceArea.Value, nameof(patch.SurfaceArea));
            SurfaceArea = patch.SurfaceArea.Value;
        }

        if (patch.InsuredValue.HasValue)
        {
            Guard.Positive(patch.InsuredValue.Value, nameof(patch.InsuredValue));
            InsuredValue = patch.InsuredValue.Value;
        }

        if (patch.Address.HasValue)
            Address = patch.Address.Value;

        if (patch.FloodZone.HasValue)
            FloodZone = patch.FloodZone.Value;

        if (patch.EarthquakeRiskZone.HasValue)
            EarthquakeRiskZone = patch.EarthquakeRiskZone.Value;
    }



    public void LinkPolicy(Guid policyId)
    {
        if (policyId == Guid.Empty) throw new DomainException("policyId is required.");
        if (!_policyIds.Contains(policyId)) _policyIds.Add(policyId);
    }
}