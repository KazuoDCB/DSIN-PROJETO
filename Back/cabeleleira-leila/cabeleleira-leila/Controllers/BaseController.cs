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
    public IActionResult GetAll(
        [FromQuery] int size = 12,
        [FromQuery] int page = 1)
    {
        if (size <= 0 || page <= 0) return BadRequest("Size e page devem ser maiores que zero.");

        OperationResult<List<TResponse>> result = Service.GetAll(page, size);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(TKey id)
    {
        OperationResult<TResponse> result = Service.GetById(id);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(TKey id)
    {
        OperationResult result = Service.Delete(id);

        return StatusCode((int)result.StatusCode, result);
    }
}
