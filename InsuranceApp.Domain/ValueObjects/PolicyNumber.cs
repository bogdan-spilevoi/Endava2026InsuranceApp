using InsuranceApp.Domain.Common;

namespace InsuranceApp.Domain.ValueObjects;

public readonly record struct PolicyNumber
{
    public string Value { get; }
    public PolicyNumber(string value)
    {
        Guard.NotNullOrWhiteSpace(value, nameof(PolicyNumber), 40);
        Value = value.Trim().ToUpperInvariant();
    }
    public override string ToString() => Value;
}