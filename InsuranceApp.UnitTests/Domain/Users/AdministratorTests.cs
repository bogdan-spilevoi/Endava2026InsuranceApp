using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Users;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.Users;

public sealed class AdministratorTests
{
    [Fact]
    public void Ctor_ShouldCreateAdmin_WhenValid()
    {
        var a = DomainFactory.Admin(AdminRole.Manager);

        a.Name.Should().Be("Admin Name");
        a.Role.Should().Be(AdminRole.Manager);
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenNameInvalid()
    {
        var act = () => new Administrator(TestIds.New(), " ", DomainFactory.Email(), AdminRole.Admin);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_ShouldUpdateAllFields_AndValidateName()
    {
        var a = DomainFactory.Admin();
        var bad = () => a.Update("", DomainFactory.Email(), AdminRole.Admin);
        bad.Should().Throw<DomainException>();

        var email = new Email("boss@example.com");
        a.Update("Boss", email, AdminRole.Manager);

        a.Name.Should().Be("Boss");
        a.Email.Should().Be(email);
        a.Role.Should().Be(AdminRole.Manager);
    }
}