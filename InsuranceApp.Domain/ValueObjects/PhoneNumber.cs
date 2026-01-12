using InsuranceApp.Domain.Common;

namespace InsuranceApp.Domain.ValueObjects;

public readonly record struct PhoneNumber
{
    public string Value { get; }
    public PhoneNumber(string value)
    {
        Guard.NotNullOrWhiteSpace(value, nameof(PhoneNumber), 32);
        Value = value.Trim();
    }
    public override string ToString() => Value;
}