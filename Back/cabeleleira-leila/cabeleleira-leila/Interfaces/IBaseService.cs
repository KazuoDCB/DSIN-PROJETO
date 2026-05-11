using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IBaseService<TRequest, TUpdateRequest, TResponse, TKey>
    where TRequest : class
    where TUpdateRequest : class
    where TResponse : class
    where TKey : notnull
{
    OperationResult<List<TResponse>> GetAll(int page, int size);
    OperationResult<TResponse> GetById(TKey id);
    OperationResult Delete(TKey id);
}
