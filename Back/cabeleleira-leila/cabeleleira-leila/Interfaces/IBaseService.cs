using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IBaseService<TRequest, TUpdateRequest, TResponse, TKey>
    where TRequest : class
    where TUpdateRequest : class
    where TResponse : class
    where TKey : notnull
{
    Task<OperationResult<List<TResponse>>> GetAllAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<OperationResult<TResponse>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<OperationResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}
