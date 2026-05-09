using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IBaseRepository<TEntity, TKey>
    where TEntity : BaseModel
    where TKey : notnull
{
    IQueryable<TEntity> Query();
    Task<List<TEntity>> GetAllAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
