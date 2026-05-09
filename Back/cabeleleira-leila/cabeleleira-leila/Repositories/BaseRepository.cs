using cabeleleira_leila.DataBase;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.EntityFrameworkCore;

namespace cabeleleira_leila.Repositories;

public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
    where TEntity : BaseModel
    where TKey : notnull
{
    protected readonly Cabeleleira_LeilaDbContext Context;

    protected BaseRepository(Cabeleleira_LeilaDbContext context)
    {
        Context = context;
    }

    public IQueryable<TEntity> Query()
    {
        return Context.Set<TEntity>().AsQueryable();
    }

    public async Task<List<TEntity>> GetAllAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        return await Context
            .Set<TEntity>()
            .AsNoTracking()
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().FindAsync([id], cancellationToken);
    }

    public async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var convertedId = Convert.ToInt64(id);

        return await Context
            .Set<TEntity>()
            .AnyAsync(entity => entity.Id == convertedId, cancellationToken);
    }

    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
