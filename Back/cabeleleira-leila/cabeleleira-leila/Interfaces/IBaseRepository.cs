using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IBaseRepository<TEntity, TKey>
    where TEntity : BaseModel
    where TKey : notnull
{
    IQueryable<TEntity> Query();
    List<TEntity> GetAll(int page, int size);
    TEntity? GetById(TKey id);
    bool Exists(TKey id);
    void Create(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
