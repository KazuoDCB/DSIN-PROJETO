using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IServicoRepository : IBaseRepository<Servico, long>
{
    List<Servico> GetByIds(IEnumerable<long> ids);
    bool NameExists(string name, long? ignoredId);
}
