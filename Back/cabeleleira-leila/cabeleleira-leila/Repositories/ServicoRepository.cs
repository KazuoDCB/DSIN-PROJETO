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

    public List<Servico> GetByIds(IEnumerable<long> ids)
    {
        var uniqueIds = ids
            .Distinct()
            .ToList();

        return Context
            .Set<Servico>()
            .Where(servico => uniqueIds.Contains(servico.Id))
            .ToList();
    }

    public bool NameExists(string name, long? ignoredId)
    {
        var normalizedName = name.Trim().ToLowerInvariant();

        return Context
            .Set<Servico>()
            .Any(servico => servico.Name.ToLower() == normalizedName && servico.Id != ignoredId);
    }
}
