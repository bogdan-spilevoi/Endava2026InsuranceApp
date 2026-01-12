using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Users;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.Users;

public sealed class BrokerTests
{
    [Fact]
    public void Ctor_ShouldCreateBroker_WhenValid()
    {
        var b = DomainFactory.Broker(status: BrokerStatus.Inactive, commission: 0.1m);

        b.BrokerCode.Should().Be("BRK-01");
        b.Name.Should().Be("Broker Name");
        b.IsActive.Should().BeFalse();
        b.CommissionPercentage.Should().Be(0.1m);
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenBrokerCodeOrNameInvalid()
    {
        var act1 = () => new Broker(TestIds.New(), "", "Name", DomainFactory.Email(), DomainFactory.Phone(), BrokerStatus.Active);
        act1.Should().Throw<DomainException>();

        var act2 = () => new Broker(TestIds.New(), "CODE", "", DomainFactory.Email(), DomainFactory.Phone(), BrokerStatus.Active);
        act2.Should().Throw<DomainException>();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenCommissionOutOfRange()
    {
        var act1 = () => DomainFactory.Broker(commission: -0.01m);
        act1.Should().Throw<DomainException>();

        var act2 = () => DomainFactory.Broker(commission: 1.01m);
        act2.Should().Throw<DomainException>();
    }

    [Fact]
    public void ActivateDeactivate_ShouldToggleStatus()
    {
        var b = DomainFactory.Broker(status: BrokerStatus.Inactive);
        b.IsActive.Should().BeFalse();

        b.Activate();
        b.IsActive.Should().BeTrue();

        b.Deactivate();
        b.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateContact_ShouldUpdateValues_AndValidateName()
    {
        var b = DomainFactory.Broker();

        var act = () => b.UpdateContact("", DomainFactory.Email(), DomainFactory.Phone());
        act.Should().Throw<DomainException>();

        var newEmail = new Email("new@example.com");
        var newPhone = new PhoneNumber("123");
        b.UpdateContact("New Name", newEmail, newPhone);

        b.Name.Should().Be("New Name");
        b.Email.Should().Be(newEmail);
        b.Phone.Should().Be(newPhone);
    }

    [Fact]
    public void UpdateCommission_ShouldSetOrClear_AndValidateRange()
    {
        var b = DomainFactory.Broker();

        var bad = () => b.UpdateCommission(2m);
        bad.Should().Throw<DomainException>();

        b.UpdateCommission(0.25m);
        b.CommissionPercentage.Should().Be(0.25m);

        b.UpdateCommission(null);
        b.CommissionPercentage.Should().BeNull();
    }
}