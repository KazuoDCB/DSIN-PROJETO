using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using MapsterMapper;

namespace cabeleleira_leila.Services;

public abstract class BaseService<TEntity, TRequest, TUpdateRequest, TResponse, TKey> :
    IBaseService<TRequest, TUpdateRequest, TResponse, TKey>
    where TEntity : BaseModel
    where TRequest : class
    where TUpdateRequest : class
    where TResponse : class
    where TKey : notnull
{
    protected readonly IBaseRepository<TEntity, TKey> Repository;
    protected readonly IMapper Mapper;

    protected BaseService(IBaseRepository<TEntity, TKey> repository, IMapper mapper)
    {
        Repository = repository;
        Mapper = mapper;
    }

    public virtual OperationResult<List<TResponse>> GetAll(
        int page,
        int size)
    {
        var entities = Repository.GetAll(page, size);

        return OperationResult<List<TResponse>>.Ok(Mapper.Map<List<TResponse>>(entities));
    }

    public virtual OperationResult<TResponse> GetById(TKey id)
    {
        var entity = Repository.GetById(id);

        if (entity is null) return OperationResult<TResponse>.NotFound(NotFoundError());

        return OperationResult<TResponse>.Ok(Mapper.Map<TResponse>(entity));
    }

    public virtual OperationResult Delete(TKey id)
    {
        var entity = Repository.GetById(id);

        if (entity is null) return OperationResult.NotFound(NotFoundError());

        Repository.Delete(entity);

        return OperationResult.Ok();
    }

    protected ErrorMessage NotFoundError()
    {
        return ErrorMessage.CreateErrorMessage(typeof(TEntity).Name, "Nao encontrado.");
    }

    protected static ErrorMessage Error(string property, string message)
    {
        return ErrorMessage.CreateErrorMessage(property, message);
    }
}
