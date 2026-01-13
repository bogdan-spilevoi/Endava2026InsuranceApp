using InsuranceApp.Domain.Clients;
using InsuranceApp.Domain.Repositories;
using InsuranceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Repositories;

public class ClientRepository(InsuranceAppDbContext db) : GenericRepository<Client>(db), IClientRepository
{
    public async Task<IReadOnlyList<Client>> SearchClientByMatch(string name, string identifier, int pageIndex = 0, int pageCount = 10, CancellationToken ct = default)
    {
        IQueryable<Client> query = _dbSet;

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(identifier))
            query = query.Where(x => x.IdentificationNumber.Contains(identifier));

        return await query
            .OrderBy(x => x.Id)
            .Skip(pageIndex * pageCount)
            .Take(pageCount)
            .ToListAsync(ct);
    }
}