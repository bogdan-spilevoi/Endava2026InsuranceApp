namespace InsuranceApp.Domain.Common;

public sealed class County : Entity<Guid>
{
    private readonly List<City> _cities = new();
    public string Name { get; private set; }
    public Guid CountryId { get; private set; }
    public IReadOnlyCollection<City> Cities => _cities.AsReadOnly();

    public County(Guid id, string name, Guid countryId) : base(id)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 120);
        Name = name.Trim();
        CountryId = countryId;
    }

    public City AddCity(Guid cityId, string name, string? countyCode = null)
    {
        var city = new City(cityId, name, Id, countyCode);
        _cities.Add(city);
        return city;
    }
}