using cabeleleira_leila.DTO;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.AspNetCore.Mvc;

namespace cabeleleira_leila.Controllers;

[ApiController]
[Route("api/servicos")]
public class ServicosController :
    BaseController<ServicoRequestDto, ServicoUpdateRequestDto, ServicoResponseDto, IServicoService, long>
{
    private readonly IServicoService _servicoService;

    public ServicosController(IServicoService servicoService) : base(servicoService)
    {
        _servicoService = servicoService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ServicoRequestDto request, CancellationToken cancellationToken)
    {
        OperationResult<ServicoResponseDto> result = await _servicoService.CreateAsync(request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] ServicoUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        OperationResult<ServicoResponseDto> result = await _servicoService.UpdateAsync(id, request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }
}
