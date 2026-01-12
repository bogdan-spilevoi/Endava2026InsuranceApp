using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Metadata;

namespace InsuranceApp.UnitTests.Domain.Metadata;

public sealed class RiskFactorConfigurationTests
{
    [Fact]
    public void Ctor_ShouldCreateCityRiskFactor_WhenValid()
    {
        var cityId = TestIds.New();
        var rf = DomainFactory.RiskByCity(cityId, 0.05m, active: true);

        rf.Level.Should().Be(RiskFactorLevel.City);
        rf.ReferenceId.Should().Be(cityId);
        rf.BuildingType.Should().BeNull();
        rf.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Ctor_ShouldCreateBuildingTypeRiskFactor_WhenValid()
    {
        var rf = DomainFactory.RiskByBuildingType(BuildingType.Office, -0.02m, active: true);

        rf.Level.Should().Be(RiskFactorLevel.BuildingType);
        rf.ReferenceId.Should().BeNull();
        rf.BuildingType.Should().Be(BuildingType.Office);
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenAdjustmentTooLow()
    {
        var act = () => DomainFactory.RiskByCity(TestIds.New(), -1m, active: true);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenBuildingTypeLevelMissingBuildingType()
    {
        var act = () => new RiskFactorConfiguration(
            id: TestIds.New(),
            level: RiskFactorLevel.BuildingType,
            referenceId: null,
            buildingType: null,
            adjustmentPercentage: 0.01m,
            isActive: true);

        act.Should().Throw<DomainException>().WithMessage("*BuildingType is required*");
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenGeographyLevelMissingReferenceId()
    {
        var act = () => new RiskFactorConfiguration(
            id: TestIds.New(),
            level: RiskFactorLevel.City,
            referenceId: null,
            buildingType: null,
            adjustmentPercentage: 0.01m,
            isActive: true);

        act.Should().Throw<DomainException>().WithMessage("*ReferenceId is required*");
    }

    [Fact]
    public void ActivateDeactivate_ShouldToggle()
    {
        var rf = DomainFactory.RiskByCity(TestIds.New(), 0.01m, active: false);
        rf.IsActive.Should().BeFalse();

        rf.Activate();
        rf.IsActive.Should().BeTrue();

        rf.Deactivate();
        rf.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateAdjustment_ShouldUpdate_AndValidate()
    {
        var rf = DomainFactory.RiskByCity(TestIds.New(), 0.01m, active: true);

        rf.UpdateAdjustment(0.02m);
        rf.AdjustmentPercentage.Should().Be(0.02m);

        var act = () => rf.UpdateAdjustment(-1m);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenUnsupportedLevel()
    {
        // Create an invalid enum value to hit default branch
        var invalid = (RiskFactorLevel)999;

        var act = () => new RiskFactorConfiguration(
            id: TestIds.New(),
            level: invalid,
            referenceId: TestIds.New(),
            buildingType: null,
            adjustmentPercentage: 0.01m,
            isActive: true);

        act.Should().Throw<DomainException>().WithMessage("*Unsupported*");
    }
}