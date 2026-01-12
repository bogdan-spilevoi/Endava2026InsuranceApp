using InsuranceApp.Domain.Common;

namespace InsuranceApp.Domain.ValueObjects;

public readonly record struct Address
{
    public string Street { get; }
    public string Number { get; }
    public string? Additional { get; }

    public Address(string street, string number, string? additional = null)
    {
        Guard.NotNullOrWhiteSpace(street, nameof(Street), 200);
        Guard.NotNullOrWhiteSpace(number, nameof(Number), 30);
        Street = street.Trim();
        Number = number.Trim();
        Additional = string.IsNullOrWhiteSpace(additional) ? null : additional.Trim();
    }

    public override string ToString()
        => Additional is null ? $"{Street}, {Number}" : $"{Street}, {Number}, {Additional}";
}