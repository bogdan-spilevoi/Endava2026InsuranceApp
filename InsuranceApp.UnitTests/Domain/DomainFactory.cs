using InsuranceApp.Domain.Buildings;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Metadata;
using InsuranceApp.Domain.Policies;
using InsuranceApp.Domain.Users;
using InsuranceApp.Domain.ValueObjects;
using InsuranceApp.UnitTests.Domain.Buildings;
using InsuranceApp.UnitTests.Domain.Policies;

namespace InsuranceApp.UnitTests.Domain;

internal static class DomainFactory
{
    public static Address Address() => new Address("Street", "10A", "Apt 3");
    public static Email Email() => new Email("test@example.com");
    public static PhoneNumber Phone() => new PhoneNumber("+40-700-000-000");

    public static Broker Broker(BrokerStatus status = BrokerStatus.Active, decimal? commission = null)
        => new Broker(
            id: TestIds.New(),
            brokerCode: "BRK-01",
            name: "Broker Name",
            email: Email(),
            phone: Phone(),
            status: status,
            commissionPercentage: commission);

    public static Administrator Admin(AdminRole role = AdminRole.Admin)
        => new Administrator(TestIds.New(), "Admin Name", Email(), role);

    public static Currency Currency(bool active = true)
        => new Currency(TestIds.New(), "RON", "Romanian Leu", active);

    public static FeeConfiguration Fee(
        decimal percentage,
        DateOnly from,
        DateOnly? to,
        bool active = true,
        FeeConfigurationType type = FeeConfigurationType.AdminFee)
        => new FeeConfiguration(TestIds.New(), "Fee Name", type, percentage, from, to, active);

    public static RiskFactorConfiguration RiskByCity(Guid cityId, decimal adj, bool active = true)
        => new RiskFactorConfiguration(TestIds.New(), RiskFactorLevel.City, cityId, null, adj, active);

    public static RiskFactorConfiguration RiskByBuildingType(BuildingType type, decimal adj, bool active = true)
        => new RiskFactorConfiguration(TestIds.New(), RiskFactorLevel.BuildingType, null, type, adj, active);

        public static CreateBuildingParams CreateBuildingParams(TestingBuildingDetails? d = null)
        => d ?? new TestingBuildingDetails();

    public static UpdateBuildingParams UpdateBuildingDetails(TestingBuildingDetails? d = null)
        => d ?? new TestingBuildingDetails();

    public static PatchBuildingParams PatchBuildingDetails(TestingBuildingDetails? d = null)
        => d ?? new TestingBuildingDetails();

    public static Building Building(CreateBuildingParams? p = null, Guid? id = null)
        => new Building(id ?? TestIds.New(), p ?? CreateBuildingParams());

    public static TestingPolicyDetails PolicyDraftDetails(
        Action<TestingPolicyDetails>? mutate = null)
    {
        var d = new TestingPolicyDetails();
        mutate?.Invoke(d);
        return d;
    }

    public static Policy DraftPolicy(
        TestingPolicyDetails? d = null,
        Guid? id = null)
        => Policy.CreateDraft(
            id ?? TestIds.New(),
            d ?? new TestingPolicyDetails());
}