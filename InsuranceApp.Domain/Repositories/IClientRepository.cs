using InsuranceApp.Domain.Clients;

namespace InsuranceApp.Domain.Repositories;

public interface IClientRepository : IRepository<Client>
{
    Task<IReadOnlyList<Client>> SearchClientByMatch(string name, string identifier, int pageIndex = 0, int pageCount = 10, CancellationToken ct = default);
}