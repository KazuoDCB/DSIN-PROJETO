using cabeleleira_leila.DTO;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.AspNetCore.Mvc;

namespace cabeleleira_leila.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController :
    BaseController<ClienteRequestDto, ClienteUpdateRequestDto, ClienteResponseDto, IClienteService, long>
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService) : base(clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    public IActionResult Create([FromBody] ClienteRequestDto request)
    {
        OperationResult<ClienteResponseDto> result = _clienteService.Create(request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:long}")]
    public IActionResult Update(
        long id,
        [FromBody] ClienteUpdateRequestDto request)
    {
        OperationResult<ClienteResponseDto> result = _clienteService.Update(id, request);

        return StatusCode((int)result.StatusCode, result);
    }
}
