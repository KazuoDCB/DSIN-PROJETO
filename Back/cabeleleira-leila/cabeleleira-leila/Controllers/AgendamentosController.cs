using cabeleleira_leila.DTO;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.AspNetCore.Mvc;

namespace cabeleleira_leila.Controllers;

[ApiController]
[Route("api/agendamentos")]
public class AgendamentosController :
    BaseController<SchedulingRequestDto, SchedulingUpdateRequestDto, SchedulingResponseDto, ISchedulingService, long>
{
    private readonly ISchedulingService _schedulingService;

    public AgendamentosController(ISchedulingService schedulingService) : base(schedulingService)
    {
        _schedulingService = schedulingService;
    }

    [HttpGet("cliente/{clienteId:long}")]
    public async Task<IActionResult> GetByCliente(
        long clienteId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        CancellationToken cancellationToken)
    {
        OperationResult<List<SchedulingResponseDto>> result = await _schedulingService.GetByClienteAsync(
            clienteId,
            dataInicio,
            dataFim,
            cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("sugestao")]
    public async Task<IActionResult> GetSameWeekSuggestion(
        [FromQuery] long clienteId,
        [FromQuery] DateTime dataHora,
        CancellationToken cancellationToken)
    {
        OperationResult<SchedulingResponseDto> result = await _schedulingService.GetSameWeekSuggestionAsync(
            clienteId,
            dataHora,
            cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SchedulingRequestDto request, CancellationToken cancellationToken)
    {
        OperationResult<SchedulingResponseDto> result = await _schedulingService.CreateAsync(request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] SchedulingUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        OperationResult<SchedulingResponseDto> result = await _schedulingService.UpdateAsync(id, request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:long}/admin")]
    public async Task<IActionResult> AdminUpdate(
        long id,
        [FromBody] SchedulingUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        OperationResult<SchedulingResponseDto> result = await _schedulingService.AdminUpdateAsync(id, request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(
        long id,
        [FromBody] SchedulingStatusUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        OperationResult<SchedulingResponseDto> result = await _schedulingService.UpdateStatusAsync(id, request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }
}
