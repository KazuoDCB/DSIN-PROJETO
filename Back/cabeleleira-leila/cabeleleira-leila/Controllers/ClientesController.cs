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
    public async Task<IActionResult> Create([FromBody] ClienteRequestDto request, CancellationToken cancellationToken)
    {
        OperationResult<ClienteResponseDto> result = await _clienteService.CreateAsync(request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] ClienteUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        OperationResult<ClienteResponseDto> result = await _clienteService.UpdateAsync(id, request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }
}
