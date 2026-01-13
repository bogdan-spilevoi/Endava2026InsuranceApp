using InsuranceApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InsuranceApp.Infrastructure.Persistence.Converters;

public sealed class AddressConverter : ValueConverter<Address?, string?>
{
    public AddressConverter()
        : base(
            convertToProviderExpression: a => Serialize(a),
            convertFromProviderExpression: s => Deserialize(s))
    { }

    private static string? Serialize(Address? a)
        => a is null
            ? null
            : $"{a.Value.Street}|{a.Value.Number}|{a.Value.Additional}";

    private static Address? Deserialize(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;

        var parts = s.Split('|', 3);
        var street = parts.Length > 0 ? parts[0] : "";
        var number = parts.Length > 1 ? parts[1] : "";
        var additional = parts.Length > 2 ? parts[2] : null;

        return new Address(street, number, additional);
    }
}
