using FluentAssertions;
using InsuranceApp.Domain.Clients;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InsuranceApp.UnitTests.Infrastructure.Persistence.Configurations;

public sealed class ClientConfigurationTests
{
    [Fact]
    public void Configure_maps_client_correctly()
    {
        var model = EfModelTestHelper.BuildModel<Client>(b =>
            new ClientConfiguration().Configure(b));

        var entity = model.FindEntityType(typeof(Client));
        entity.Should().NotBeNull();
        entity!.GetTableName().Should().Be("Clients");

        var pk = entity.FindPrimaryKey();
        pk.Should().NotBeNull();
        pk.Properties.Should().ContainSingle();
        pk.Properties[0].Name.Should().Be(nameof(Client.Id));

        var name = entity.FindProperty(nameof(Client.Name));
        name.Should().NotBeNull();
        name.IsNullable.Should().BeFalse();
        name.GetMaxLength().Should().Be(200);

        var idNo = entity.FindProperty(nameof(Client.IdentificationNumber));
        idNo.Should().NotBeNull();
        idNo.IsNullable.Should().BeFalse();
        idNo.GetMaxLength().Should().Be(32);

        var type = entity.FindProperty(nameof(Client.Type));
        type.Should().NotBeNull();
        type.ClrType.Should().Be<ClientType>();
        type.Should().NotBeNull();

        var email = entity.FindProperty(nameof(Client.Email));
        email.Should().NotBeNull();
        email.IsNullable.Should().BeTrue();
        email.GetMaxLength().Should().Be(254);
        email.GetColumnName(StoreObjectIdentifier.Table("Clients", null)).Should().Be("Email");
        email.Should().NotBeNull();

        var phone = entity.FindProperty(nameof(Client.Phone));
        phone.Should().NotBeNull();
        phone.IsNullable.Should().BeTrue();
        phone.GetMaxLength().Should().Be(32);
        phone.GetColumnName(StoreObjectIdentifier.Table("Clients", null)).Should().Be("Phone");
        phone.Should().NotBeNull();

        var address = entity.FindProperty(nameof(Client.PrimaryAddress));
        address.Should().NotBeNull();
        address.IsNullable.Should().BeTrue();
        address.GetColumnName(StoreObjectIdentifier.Table("Clients", null)).Should().Be("PrimaryAddress");
        address.GetValueConverter().Should().NotBeNull();

        entity.GetIndexes().Should().Contain(ix =>
            ix.IsUnique &&
            ix.Properties.Count == 1 &&
            ix.Properties[0].Name == nameof(Client.IdentificationNumber));
    }
}
