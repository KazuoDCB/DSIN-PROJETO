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

    public List<TEntity> GetAll(int page, int size)
    {
        return Context
            .Set<TEntity>()
            .AsNoTracking()
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
    }

    public TEntity? GetById(TKey id)
    {
        return Context.Set<TEntity>().Find(id);
    }

    public bool Exists(TKey id)
    {
        var convertedId = Convert.ToInt64(id);

        return Context
            .Set<TEntity>()
            .Any(entity => entity.Id == convertedId);
    }

    public void Create(TEntity entity)
    {
        Context.Set<TEntity>().Add(entity);
        Context.SaveChanges();
    }

    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
        Context.SaveChanges();
    }

    public void Delete(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
        Context.SaveChanges();
    }
}
