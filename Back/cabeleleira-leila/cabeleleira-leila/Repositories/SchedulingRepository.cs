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

    public List<Scheduling> GetAllWithDetails(int page, int size)
    {
        return SchedulingQuery()
            .AsNoTracking()
            .OrderBy(scheduling => scheduling.DataHora)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
    }

    public List<Scheduling> GetByClienteWithDetails(
        long clienteId,
        DateTime? start,
        DateTime? end)
    {
        var query = SchedulingQuery()
            .AsNoTracking()
            .Where(scheduling => scheduling.ClienteId == clienteId);

        if (start.HasValue) query = query.Where(scheduling => scheduling.DataHora >= start.Value);
        if (end.HasValue) query = query.Where(scheduling => scheduling.DataHora <= end.Value);

        return query
            .OrderByDescending(scheduling => scheduling.DataHora)
            .ToList();
    }

    public Scheduling? GetWithDetails(
        long id,
        bool asNoTracking)
    {
        var query = SchedulingQuery();

        if (asNoTracking) query = query.AsNoTracking();

        return query.FirstOrDefault(scheduling => scheduling.Id == id);
    }

    public Scheduling? GetSameWeek(
        long clienteId,
        DateTime dataHora)
    {
        var weekStart = dataHora.Date.AddDays(-(int)dataHora.DayOfWeek);
        var weekEnd = weekStart.AddDays(7);

        return SchedulingQuery()
            .AsNoTracking()
            .Where(scheduling => scheduling.ClienteId == clienteId)
            .Where(scheduling => scheduling.DataHora >= weekStart && scheduling.DataHora < weekEnd)
            .OrderBy(scheduling => scheduling.DataHora)
            .FirstOrDefault();
    }

    public bool HasSchedulingAt(DateTime dataHora, long? ignoredId)
    {
        return Context
            .Set<Scheduling>()
            .Any(scheduling => scheduling.DataHora == dataHora && scheduling.Id != ignoredId);
    }

    private IQueryable<Scheduling> SchedulingQuery()
    {
        return Context
            .Set<Scheduling>()
            .Include(scheduling => scheduling.User)
            .Include(scheduling => scheduling.Servicos);
    }
}
