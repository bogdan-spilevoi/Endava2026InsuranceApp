using InsuranceApp.Domain.Common;

namespace InsuranceApp.Domain.Metadata;

public sealed class Currency : AggregateRoot<Guid>
{
    public string Code { get; private set; } // RON, EUR
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    public Currency(Guid id, string code, string name, bool isActive) : base(id)
    {
        Guard.NotNullOrWhiteSpace(code, nameof(code), 3);
        Guard.NotNullOrWhiteSpace(name, nameof(name), 80);
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        IsActive = isActive;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}