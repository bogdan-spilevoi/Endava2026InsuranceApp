namespace InsuranceApp.Domain.Common;

public sealed class Country : AggregateRoot<Guid>
{
    private readonly List<County> _counties = new();
    public string Name { get; private set; }
    public IReadOnlyCollection<County> Counties => _counties.AsReadOnly();

    public Country(Guid id, string name) : base(id)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 120);
        Name = name.Trim();
    }

    public County AddCounty(Guid countyId, string name)
    {
        var county = new County(countyId, name, Id);
        _counties.Add(county);
        return county;
    }
}