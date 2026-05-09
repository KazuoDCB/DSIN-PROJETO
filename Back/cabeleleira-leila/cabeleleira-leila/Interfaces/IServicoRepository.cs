using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IServicoRepository : IBaseRepository<Servico, long>
{
    Task<List<Servico>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, long? ignoredId, CancellationToken cancellationToken = default);
}
