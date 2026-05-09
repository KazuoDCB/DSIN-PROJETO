using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.AspNetCore.Mvc;

namespace cabeleleira_leila.Controllers;

public abstract class BaseController<TRequest, TUpdateRequest, TResponse, TService, TKey> : ControllerBase
    where TRequest : class
    where TUpdateRequest : class
    where TResponse : class
    where TService : IBaseService<TRequest, TUpdateRequest, TResponse, TKey>
    where TKey : notnull
{
    protected readonly TService Service;

    protected BaseController(TService service)
    {
        Service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int size = 12,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        if (size <= 0 || page <= 0) return BadRequest("Size e page devem ser maiores que zero.");

        OperationResult<List<TResponse>> result = await Service.GetAllAsync(page, size, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(TKey id, CancellationToken cancellationToken)
    {
        OperationResult<TResponse> result = await Service.GetByIdAsync(id, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(TKey id, CancellationToken cancellationToken)
    {
        OperationResult result = await Service.DeleteAsync(id, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }
}
