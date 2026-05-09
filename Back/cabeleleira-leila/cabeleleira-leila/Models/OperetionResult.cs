using System.Net;

namespace cabeleleira_leila.Models;

public class OperationResult
{
    private readonly List<ErrorMessage> _erros = [];
    public bool Success => !_erros.Any();
    public HttpStatusCode StatusCode { get; protected init; }
    public IReadOnlyList<ErrorMessage> Errors => _erros.ToList();

    protected OperationResult(HttpStatusCode statusCode, IEnumerable<ErrorMessage>? erros = null)
    {
        StatusCode = statusCode;
        if (erros is null) return;

        _erros.AddRange(erros.Where(e => e is not null));
    }

    public static OperationResult Ok() => new OperationResult(HttpStatusCode.OK);
    public static OperationResult Created() => new OperationResult(HttpStatusCode.Created);
    public static OperationResult Fail(IEnumerable<ErrorMessage> erros) => new OperationResult(HttpStatusCode.BadRequest, erros);
    public static OperationResult Fail(ErrorMessage error) => new OperationResult(HttpStatusCode.BadRequest, [error]);
    public static OperationResult NotFound(ErrorMessage error) => new OperationResult(HttpStatusCode.NotFound, [error]);
    public static OperationResult Unauthorized(ErrorMessage error) => new OperationResult(HttpStatusCode.Unauthorized, [error]);
    public static OperationResult Forbidden(ErrorMessage error) => new OperationResult(HttpStatusCode.Forbidden, [error]);
    public static OperationResult UnprocessableEntity(IEnumerable<ErrorMessage> erros) => new OperationResult(HttpStatusCode.UnprocessableEntity, erros);
    public static OperationResult UnprocessableEntity(ErrorMessage erros) => new OperationResult(HttpStatusCode.UnprocessableEntity, [erros]);
    public static OperationResult FatalError(ErrorMessage error) => new OperationResult(HttpStatusCode.InternalServerError, [error]);
}
public class OperationResult<T> : OperationResult
{
    public T? Data { get; private set; }

    private OperationResult(HttpStatusCode statusCode, T? data = default, IEnumerable<ErrorMessage>? erros = null) : base(statusCode, erros)
    {
        Data = data;
    }

    public static OperationResult<T> Ok(T data) => new OperationResult<T>(HttpStatusCode.OK, data);
    public static OperationResult<T> Created(T data) => new OperationResult<T>(HttpStatusCode.Created, data);
    public static new OperationResult<T> Fail(IEnumerable<ErrorMessage> erros) => new OperationResult<T>(HttpStatusCode.BadRequest, default, erros);
    public static new OperationResult<T> Fail(ErrorMessage error) => new OperationResult<T>(HttpStatusCode.BadRequest, default, [error]);
    public static new OperationResult<T> NotFound(ErrorMessage error) => new OperationResult<T>(HttpStatusCode.NotFound, default, [error]);
    public static new OperationResult<T> Unauthorized(ErrorMessage error) => new OperationResult<T>(HttpStatusCode.Unauthorized, default, [error]);
    public static new OperationResult<T> Forbidden(ErrorMessage error) => new OperationResult<T>(HttpStatusCode.Forbidden, default, [error]);
    public static new OperationResult<T> UnprocessableEntity(IEnumerable<ErrorMessage> erros) => new OperationResult<T>(HttpStatusCode.UnprocessableEntity, default, erros);
    public static new OperationResult<T> UnprocessableEntity(ErrorMessage erros) => new OperationResult<T>(HttpStatusCode.UnprocessableEntity, default, [erros]);
}
