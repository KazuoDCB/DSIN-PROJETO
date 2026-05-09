using cabeleleira_leila.DataBase;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.EntityFrameworkCore;

namespace cabeleleira_leila.Repositories;

public class SchedulingRepository : BaseRepository<Scheduling, long>, ISchedulingRepository
{
    public SchedulingRepository(Cabeleleira_LeilaDbContext context) : base(context)
    {
    }

    public async Task<List<Scheduling>> GetAllWithDetailsAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        return await SchedulingQuery()
            .AsNoTracking()
            .OrderBy(scheduling => scheduling.DataHora)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Scheduling>> GetByClienteWithDetailsAsync(
        long clienteId,
        DateTime? start,
        DateTime? end,
        CancellationToken cancellationToken = default)
    {
        var query = SchedulingQuery()
            .AsNoTracking()
            .Where(scheduling => scheduling.ClienteId == clienteId);

        if (start.HasValue) query = query.Where(scheduling => scheduling.DataHora >= start.Value);
        if (end.HasValue) query = query.Where(scheduling => scheduling.DataHora <= end.Value);

        return await query
            .OrderByDescending(scheduling => scheduling.DataHora)
            .ToListAsync(cancellationToken);
    }

    public async Task<Scheduling?> GetWithDetailsAsync(
        long id,
        bool asNoTracking,
        CancellationToken cancellationToken = default)
    {
        var query = SchedulingQuery();

        if (asNoTracking) query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(scheduling => scheduling.Id == id, cancellationToken);
    }

    public async Task<Scheduling?> GetSameWeekAsync(
        long clienteId,
        DateTime dataHora,
        CancellationToken cancellationToken = default)
    {
        var weekStart = dataHora.Date.AddDays(-(int)dataHora.DayOfWeek);
        var weekEnd = weekStart.AddDays(7);

        return await SchedulingQuery()
            .AsNoTracking()
            .Where(scheduling => scheduling.ClienteId == clienteId)
            .Where(scheduling => scheduling.DataHora >= weekStart && scheduling.DataHora < weekEnd)
            .OrderBy(scheduling => scheduling.DataHora)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasSchedulingAtAsync(DateTime dataHora, long? ignoredId, CancellationToken cancellationToken = default)
    {
        return await Context
            .Set<Scheduling>()
            .AnyAsync(scheduling => scheduling.DataHora == dataHora && scheduling.Id != ignoredId, cancellationToken);
    }

    private IQueryable<Scheduling> SchedulingQuery()
    {
        return Context
            .Set<Scheduling>()
            .Include(scheduling => scheduling.Cliente)
            .Include(scheduling => scheduling.Servicos);
    }
}
