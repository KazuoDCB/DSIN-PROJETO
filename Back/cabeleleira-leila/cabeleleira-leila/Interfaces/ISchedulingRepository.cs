using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface ISchedulingRepository : IBaseRepository<Scheduling, long>
{
    List<Scheduling> GetAllWithDetails(int page, int size);
    List<Scheduling> GetByClienteWithDetails(long clienteId, DateTime? start, DateTime? end);
    Scheduling? GetWithDetails(long id, bool asNoTracking);
    Scheduling? GetSameWeek(long clienteId, DateTime dataHora);
    bool HasSchedulingAt(DateTime dataHora, long? ignoredId);
}
