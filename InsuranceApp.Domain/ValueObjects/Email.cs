using InsuranceApp.Domain.Common;

namespace InsuranceApp.Domain.ValueObjects;

public readonly record struct Email
{
    public string Value { get; }
    public Email(string value)
    {
        Guard.NotNullOrWhiteSpace(value, nameof(Email), 254);
        if (!value.Contains('@') || value.StartsWith('@') || value.EndsWith('@'))
            throw new DomainException("Invalid email format.");
        Value = value.Trim();
    }
    public override string ToString() => Value;
}