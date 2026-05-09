using cabeleleira_leila.DataBase;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.EntityFrameworkCore;

namespace cabeleleira_leila.Repositories;

public class ServicoRepository : BaseRepository<Servico, long>, IServicoRepository
{
    public ServicoRepository(Cabeleleira_LeilaDbContext context) : base(context)
    {
    }

    public async Task<List<Servico>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var uniqueIds = ids
            .Distinct()
            .ToList();

        return await Context
            .Set<Servico>()
            .Where(servico => uniqueIds.Contains(servico.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, long? ignoredId, CancellationToken cancellationToken = default)
    {
        return await Context
            .Set<Servico>()
            .AnyAsync(servico => servico.Name == name && servico.Id != ignoredId, cancellationToken);
    }
}
