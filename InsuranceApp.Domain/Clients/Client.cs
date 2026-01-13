using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Repositories;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Domain.Clients;

public sealed class Client : AggregateRoot<Guid>
{
    private readonly List<Guid> _buildingIds = new();
    private readonly List<Guid> _policyIds = new();

    public ClientType Type { get; private set; }
    public string Name { get; private set; }
    public string IdentificationNumber { get; private set; }
    public Email? Email { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public Address? PrimaryAddress { get; private set; }

    public IReadOnlyCollection<Guid> BuildingIds => _buildingIds.AsReadOnly();
    public IReadOnlyCollection<Guid> PolicyIds => _policyIds.AsReadOnly();

    public Client(
        Guid id,
        ClientType type,
        string name,
        string identificationNumber,
        Email? email,
        PhoneNumber? phone,
        Address? primaryAddress) : base(id)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
        Guard.NotNullOrWhiteSpace(identificationNumber, nameof(identificationNumber), 32);

        Type = type;
        Name = name.Trim();
        IdentificationNumber = identificationNumber.Trim().ToUpperInvariant();
        Email = email;
        Phone = phone;
        PrimaryAddress = primaryAddress;
    }

    public void UpdateDetails(string name, Email? email, PhoneNumber? phone, Address? primaryAddress)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
        Name = name.Trim();
        Email = email;
        Phone ??= phone;
        PrimaryAddress = primaryAddress;
    }

    public void PatchDetails(string? name = null, Email? email = null, PhoneNumber? phone = null, Address? primaryAddress = null)
    {
        if (name is not null)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
            Name = name.Trim();
        }

        if (email is not null) Email = email;
        if (phone is not null) Phone = phone;
        if (primaryAddress is not null) PrimaryAddress = primaryAddress;
    }

    public void ChangeIdentificationNumber(string newId, string reason)
    {
        Guard.NotNullOrWhiteSpace(newId, nameof(newId), 32);
        Guard.NotNullOrWhiteSpace(reason, nameof(reason), 300);

        newId = newId.Trim().ToUpperInvariant();
        if (newId == IdentificationNumber) return;

        IdentificationNumber = newId;
    }

    public void LinkBuilding(Guid buildingId)
    {
        if (buildingId == Guid.Empty) throw new DomainException("buildingId is required.");
        if (!_buildingIds.Contains(buildingId)) _buildingIds.Add(buildingId);
    }

    public void LinkPolicy(Guid policyId)
    {
        if (policyId == Guid.Empty) throw new DomainException("policyId is required.");
        if (!_policyIds.Contains(policyId)) _policyIds.Add(policyId);
    }
}