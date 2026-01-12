using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.Buildings;

public sealed class BuildingTests
{
    [Fact]
    public void Ctor_ShouldCreateBuilding_WhenValid()
    {
        var b = DomainFactory.Building();

        b.OwnerClientId.Should().NotBe(Guid.Empty);
        b.CityId.Should().NotBe(Guid.Empty);
        b.Address.Should().Be(DomainFactory.Address());
        b.InsuredValue.Should().BePositive();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenOwnerClientIdEmpty()
    {
        var p = DomainFactory.CreateBuildingParams(new TestingBuildingDetails
        {
            OwnerClientId = Guid.Empty
        });

        var act = () => DomainFactory.Building(p);
        act.Should().Throw<DomainException>().WithMessage("*OwnerClientId*");
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenCityIdEmpty()
    {
        var p = DomainFactory.CreateBuildingParams(new TestingBuildingDetails
        {
            CityId = Guid.Empty
        });

        var act = () => DomainFactory.Building(p);
        act.Should().Throw<DomainException>().WithMessage("*CityId*");
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenConstructionYearOutOfRange()
    {
        var tooOld = DomainFactory.CreateBuildingParams(new TestingBuildingDetails
        {
            ConstructionYear = 1700
        });

        var act1 = () => DomainFactory.Building(tooOld);
        act1.Should().Throw<DomainException>();

        var future = DomainFactory.CreateBuildingParams(new TestingBuildingDetails
        {
            ConstructionYear = DateTime.UtcNow.Year + 1
        });

        var act2 = () => DomainFactory.Building(future);
        act2.Should().Throw<DomainException>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenFloorsOutOfRange()
    {
        var neg = DomainFactory.CreateBuildingParams(new TestingBuildingDetails
        {
            NumberOfFloors = -1
        });

        var act1 = () => DomainFactory.Building(neg);
        act1.Should().Throw<DomainException>();

        var tooHigh = DomainFactory.CreateBuildingParams(new TestingBuildingDetails
        {
            NumberOfFloors = 1000
        });

        var act2 = () => DomainFactory.Building(tooHigh);
        act2.Should().Throw<DomainException>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenSurfaceAreaOrInsuredValueNotPositive()
    {
        var surface0 = DomainFactory.CreateBuildingParams(new TestingBuildingDetails
        {
            SurfaceArea = 0
        });

        var act1 = () => DomainFactory.Building(surface0);
        act1.Should().Throw<DomainException>();

        var insured0 = DomainFactory.CreateBuildingParams(new TestingBuildingDetails
        {
            InsuredValue = 0
        });

        var act2 = () => DomainFactory.Building(insured0);
        act2.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdateDetails_ShouldReplaceAll_WhenValid()
    {
        var b = DomainFactory.Building(DomainFactory.CreateBuildingParams());

        var newCityId = TestIds.New();
        var newAddress = new Address("New Street", "99");

        var update = DomainFactory.UpdateBuildingDetails(new TestingBuildingDetails
        {
            Address = newAddress,
            CityId = newCityId,
            ConstructionYear = 2010,
            BuildingType = BuildingType.Office,
            NumberOfFloors = 10,
            SurfaceArea = 500,
            InsuredValue = 999_999,
            FloodZone = true,
            EarthquakeRiskZone = true
        });

        b.UpdateDetails(update);

        b.Address.Should().Be(newAddress);
        b.CityId.Should().Be(newCityId);
        b.ConstructionYear.Should().Be(2010);
        b.BuildingType.Should().Be(BuildingType.Office);
        b.NumberOfFloors.Should().Be(10);
        b.SurfaceArea.Should().Be(500);
        b.InsuredValue.Should().Be(999_999);
        b.FloodZone.Should().BeTrue();
        b.EarthquakeRiskZone.Should().BeTrue();
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenCityEmpty()
    {
        var b = DomainFactory.Building(DomainFactory.CreateBuildingParams());

        var update = DomainFactory.UpdateBuildingDetails(new TestingBuildingDetails
        {
            CityId = Guid.Empty
        });

        var act = () => b.UpdateDetails(update);
        act.Should().Throw<DomainException>().WithMessage("*CityId*");
    }

    [Fact]
    public void LinkPolicy_ShouldAddOnce_AndRejectEmpty()
    {
        var b = DomainFactory.Building(DomainFactory.CreateBuildingParams());
        var pid = TestIds.New();

        b.LinkPolicy(pid);
        b.LinkPolicy(pid);

        b.PolicyIds.Should().ContainSingle(x => x == pid);

        var act = () => b.LinkPolicy(Guid.Empty);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void PatchDetails_ShouldUpdateOnlyProvidedFields()
    {
        var b = DomainFactory.Building();

        var originalCityId = b.CityId;
        var originalConstructionYear = b.ConstructionYear;
        var originalFloors = b.NumberOfFloors;
        var originalSurface = b.SurfaceArea;
        var originalEarthquake = b.EarthquakeRiskZone;

        var newAddress = new Address("Patch Street", "7");

        var patch = DomainFactory.PatchBuildingDetails(new TestingBuildingDetails
        {
            Address = newAddress,
            InsuredValue = 123_456m,
            FloodZone = true
        });

        b.PatchDetails(patch);

        b.Address.Should().Be(newAddress);
        b.InsuredValue.Should().Be(123_456m);
        b.FloodZone.Should().BeTrue();

        b.CityId.Should().Be(originalCityId);
        b.ConstructionYear.Should().Be(originalConstructionYear);
        b.NumberOfFloors.Should().Be(originalFloors);
        b.SurfaceArea.Should().Be(originalSurface);
        b.EarthquakeRiskZone.Should().Be(originalEarthquake);
    }

    [Fact]
    public void PatchDetails_ShouldThrow_WhenCityEmpty()
    {
        var b = DomainFactory.Building(DomainFactory.CreateBuildingParams());

        var patch = DomainFactory.PatchBuildingDetails(new TestingBuildingDetails
        {
            CityId = Guid.Empty
        });

        var act = () => b.PatchDetails(patch);
        act.Should().Throw<DomainException>().WithMessage("*CityId*");
    }
}
