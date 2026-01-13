using FluentAssertions;
using InsuranceApp.Domain.Clients;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.UnitTests.Domain.Clients;

public sealed class ClientTests
{
    [Fact]
    public void Ctor_ShouldTrimName_AndUppercaseIdentificationNumber_AndAssignOptionalFields()
    {
        var id = Guid.NewGuid();
        var email = new Email("  john@doe.com ");
        var phone = new PhoneNumber("  +40 700 000 000  ");
        var address = new Address("  Main St  ", "  10  ", "  Apt 2  ");

        var client = new Client(
            id,
            ClientType.Individual,
            "  John Doe  ",
            "  ab123  ",
            email,
            phone,
            address);

        client.Id.Should().Be(id);
        client.Type.Should().Be(ClientType.Individual);
        client.Name.Should().Be("John Doe");
        client.IdentificationNumber.Should().Be("AB123");
        client.Email.Should().Be(email);
        client.Phone.Should().Be(phone);
        client.PrimaryAddress.Should().Be(address);

        client.BuildingIds.Should().BeEmpty();
        client.PolicyIds.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_WhenNameIsNullOrWhitespace(string? badName)
    {
        Action act = () => _ = new Client(
            Guid.NewGuid(),
            ClientType.Company,
            badName!,
            "ID1",
            email: null,
            phone: null,
            primaryAddress: null);

        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ShouldThrow_WhenIdentificationNumberIsNullOrWhitespace(string? badId)
    {
        Action act = () => _ = new Client(
            Guid.NewGuid(),
            ClientType.Company,
            "Valid Name",
            badId!,
            email: null,
            phone: null,
            primaryAddress: null);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void UpdateDetails_ShouldTrimName_ReplaceEmail_KeepExistingPhoneIfAlreadySet_AndReplaceAddress()
    {
        var originalPhone = new PhoneNumber("111");
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Individual,
            "John",
            "ID1",
            email: new Email("john@old.com"),
            phone: originalPhone,
            primaryAddress: new Address("Street", "1"));

        var newEmail = new Email("john@new.com");
        var newPhone = new PhoneNumber("222");
        var newAddress = new Address("New Street", "99", "Floor 2");

        client.UpdateDetails("  John Updated  ", newEmail, newPhone, newAddress);

        client.Name.Should().Be("John Updated");
        client.Email.Should().Be(newEmail);

        client.Phone.Should().Be(originalPhone);

        client.PrimaryAddress.Should().Be(newAddress);
    }

    [Fact]
    public void UpdateDetails_ShouldSetPhone_WhenPhoneWasNull()
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Individual,
            "John",
            "ID1",
            email: null,
            phone: null,
            primaryAddress: null);

        var phone = new PhoneNumber("  123  ");
        client.UpdateDetails("John", email: null, phone: phone, primaryAddress: null);

        client.Phone.Should().Be(phone);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_ShouldThrow_WhenNameIsNullOrWhitespace(string? badName)
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Individual,
            "John",
            "ID1",
            email: null,
            phone: null,
            primaryAddress: null);

        Action act = () => client.UpdateDetails(badName!, email: null, phone: null, primaryAddress: null);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void PatchDetails_ShouldUpdateOnlyProvidedFields()
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Company,
            "ACME",
            "acme-01",
            email: new Email("old@acme.com"),
            phone: new PhoneNumber("111"),
            primaryAddress: new Address("Old", "1"));

        var newEmail = new Email("new@acme.com");
        var newPhone = new PhoneNumber("222");
        var newAddress = new Address("New", "2");

        client.PatchDetails(email: newEmail);
        client.Email.Should().Be(newEmail);
        client.Phone!.Value.Value.Should().Be("111");
        client.PrimaryAddress!.ToString().Should().Contain("Old");

        client.PatchDetails(phone: newPhone, primaryAddress: newAddress);
        client.Phone.Should().Be(newPhone);
        client.PrimaryAddress.Should().Be(newAddress);

        client.PatchDetails(name: "  ACME Updated  ");
        client.Name.Should().Be("ACME Updated");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void PatchDetails_ShouldThrow_WhenNameProvidedButWhitespace(string badName)
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Company,
            "ACME",
            "ACME-01",
            email: null,
            phone: null,
            primaryAddress: null);

        Action act = () => client.PatchDetails(name: badName);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ChangeIdentificationNumber_ShouldTrimUppercase_AndUpdate_WhenDifferent()
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Individual,
            "John",
            "  ab1  ",
            email: null,
            phone: null,
            primaryAddress: null);

        client.ChangeIdentificationNumber("  xy9  ", "correction");

        client.IdentificationNumber.Should().Be("XY9");
    }

    [Fact]
    public void ChangeIdentificationNumber_ShouldReturnWithoutChange_WhenSameAfterNormalization()
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Individual,
            "John",
            "AB1",
            email: null,
            phone: null,
            primaryAddress: null);

        client.ChangeIdentificationNumber("  ab1  ", "no-op");

        client.IdentificationNumber.Should().Be("AB1");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeIdentificationNumber_ShouldThrow_WhenNewIdIsInvalid(string? badNewId)
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Individual,
            "John",
            "AB1",
            email: null,
            phone: null,
            primaryAddress: null);

        Action act = () => client.ChangeIdentificationNumber(badNewId!, "reason");

        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeIdentificationNumber_ShouldThrow_WhenReasonIsInvalid(string? badReason)
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Individual,
            "John",
            "AB1",
            email: null,
            phone: null,
            primaryAddress: null);

        Action act = () => client.ChangeIdentificationNumber("NEW1", badReason!);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void LinkBuilding_ShouldThrow_WhenGuidIsEmpty()
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Company,
            "ACME",
            "ACME-01",
            email: null,
            phone: null,
            primaryAddress: null);

        Action act = () => client.LinkBuilding(Guid.Empty);

        act.Should().Throw<Exception>()
            .WithMessage("*buildingId is required*");
    }

    [Fact]
    public void LinkBuilding_ShouldAddOnce_AndIgnoreDuplicates()
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Company,
            "ACME",
            "ACME-01",
            email: null,
            phone: null,
            primaryAddress: null);

        var buildingId = Guid.NewGuid();

        client.LinkBuilding(buildingId);
        client.LinkBuilding(buildingId);

        client.BuildingIds.Should().ContainSingle().Which.Should().Be(buildingId); 
    }

    [Fact]
    public void LinkPolicy_ShouldThrow_WhenGuidIsEmpty()
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Company,
            "ACME",
            "ACME-01",
            email: null,
            phone: null,
            primaryAddress: null);

        Action act = () => client.LinkPolicy(Guid.Empty);

        act.Should().Throw<Exception>()
            .WithMessage("*policyId is required*"); 
    }

    [Fact]
    public void LinkPolicy_ShouldAddOnce_AndIgnoreDuplicates()
    {
        var client = new Client(
            Guid.NewGuid(),
            ClientType.Company,
            "ACME",
            "ACME-01",
            email: null,
            phone: null,
            primaryAddress: null);

        var policyId = Guid.NewGuid();

        client.LinkPolicy(policyId);
        client.LinkPolicy(policyId);

        client.PolicyIds.Should().ContainSingle().Which.Should().Be(policyId); 
    }
}
