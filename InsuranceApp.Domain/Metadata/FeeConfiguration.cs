using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Metadata;

public sealed class FeeConfiguration : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public FeeConfigurationType Type { get; private set; }
    public decimal Percentage { get; private set; } // 0.10m for +10%
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }

    public FeeConfiguration(
        Guid id,
        string name,
        FeeConfigurationType type,
        decimal percentage,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        bool isActive) : base(id)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 150);

        if (percentage <= -1m) throw new DomainException("Percentage must be > -1.00 (cannot reduce premium below zero via single config).");
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom) throw new DomainException("EffectiveTo must be >= EffectiveFrom.");

        Name = name.Trim();
        Type = type;
        Percentage = percentage;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        IsActive = isActive;
    }

    public bool IsEffectiveOn(DateOnly date)
        => IsActive && date >= EffectiveFrom && (!EffectiveTo.HasValue || date <= EffectiveTo.Value);

    public void UpdatePercentage(decimal percentage)
    {
        if (percentage <= -1m) throw new DomainException("Percentage must be > -1.00.");
        Percentage = percentage;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}