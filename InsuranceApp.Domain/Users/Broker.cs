

using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Domain.Users;

public sealed class Broker : AggregateRoot<Guid>
{
    public string BrokerCode { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public BrokerStatus Status { get; private set; }
    public decimal? CommissionPercentage { get; private set; }

    public bool IsActive => Status == BrokerStatus.Active;

    public Broker(
        Guid id,
        string brokerCode,
        string name,
        Email email,
        PhoneNumber? phone,
        BrokerStatus status,
        decimal? commissionPercentage = null) : base(id)
    {
        Guard.NotNullOrWhiteSpace(brokerCode, nameof(brokerCode), 30);
        Guard.NotNullOrWhiteSpace(name, nameof(name), 150);

        if (commissionPercentage.HasValue && (commissionPercentage.Value < 0m || commissionPercentage.Value > 1m))
        {
            throw new DomainException("CommissionPercentage must be between 0 and 1.");
        }

        BrokerCode = brokerCode.Trim().ToUpperInvariant();
        Name = name.Trim();
        Email = email;
        Phone = phone;
        Status = status;
        CommissionPercentage = commissionPercentage;
    }

    public void Activate() => Status = BrokerStatus.Active;
    public void Deactivate() => Status = BrokerStatus.Inactive;

    public void UpdateContact(string name, Email email, PhoneNumber? phone)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 150);
        Name = name.Trim();
        Email = email;
        Phone = phone;
    }

    public void UpdateCommission(decimal? commissionPercentage)
    {
        if (commissionPercentage.HasValue && (commissionPercentage.Value < 0m || commissionPercentage.Value > 1m))
            throw new DomainException("CommissionPercentage must be between 0 and 1.");
        CommissionPercentage = commissionPercentage;
    }
}