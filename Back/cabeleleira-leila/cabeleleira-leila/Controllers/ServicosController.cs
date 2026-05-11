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
    public IActionResult Create([FromBody] ServicoRequestDto request)
    {
        OperationResult<ServicoResponseDto> result = _servicoService.Create(request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:long}")]
    public IActionResult Update(
        long id,
        [FromBody] ServicoUpdateRequestDto request)
    {
        OperationResult<ServicoResponseDto> result = _servicoService.Update(id, request);

        return StatusCode((int)result.StatusCode, result);
    }
}
