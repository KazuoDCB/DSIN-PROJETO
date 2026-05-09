using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface ISchedulingRepository : IBaseRepository<Scheduling, long>
{
    Task<List<Scheduling>> GetAllWithDetailsAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<List<Scheduling>> GetByClienteWithDetailsAsync(long clienteId, DateTime? start, DateTime? end, CancellationToken cancellationToken = default);
    Task<Scheduling?> GetWithDetailsAsync(long id, bool asNoTracking, CancellationToken cancellationToken = default);
    Task<Scheduling?> GetSameWeekAsync(long clienteId, DateTime dataHora, CancellationToken cancellationToken = default);
    Task<bool> HasSchedulingAtAsync(DateTime dataHora, long? ignoredId, CancellationToken cancellationToken = default);
}
