namespace InsuranceApp.Domain.Common;

public sealed class City : Entity<Guid>
{
    public string Name { get; private set; }
    public string? CountyCode { get; private set; }
    public Guid CountyId { get; private set; }

    public City(Guid id, string name, Guid countyId, string? countyCode = null) : base(id)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 120);
        Name = name.Trim();
        CountyId = countyId;
        CountyCode = string.IsNullOrWhiteSpace(countyCode) ? null : countyCode.Trim().ToUpperInvariant();
    }

    public string UniqueKey => CountyCode is null ? Name : $"{Name}|{CountyCode}";
}