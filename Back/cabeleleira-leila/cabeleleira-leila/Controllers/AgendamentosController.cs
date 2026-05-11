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
    public IActionResult GetByCliente(
        long clienteId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        OperationResult<List<SchedulingResponseDto>> result = _schedulingService.GetByCliente(
            clienteId,
            dataInicio,
            dataFim);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("sugestao")]
    public IActionResult GetSameWeekSuggestion(
        [FromQuery] long clienteId,
        [FromQuery] DateTime dataHora)
    {
        OperationResult<SchedulingResponseDto> result = _schedulingService.GetSameWeekSuggestion(
            clienteId,
            dataHora);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public IActionResult Create([FromBody] SchedulingRequestDto request)
    {
        OperationResult<SchedulingResponseDto> result = _schedulingService.Create(request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:long}")]
    public IActionResult Update(
        long id,
        [FromBody] SchedulingUpdateRequestDto request)
    {
        OperationResult<SchedulingResponseDto> result = _schedulingService.Update(id, request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:long}/admin")]
    public IActionResult AdminUpdate(
        long id,
        [FromBody] SchedulingUpdateRequestDto request)
    {
        OperationResult<SchedulingResponseDto> result = _schedulingService.AdminUpdate(id, request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPatch("{id:long}/status")]
    public IActionResult UpdateStatus(
        long id,
        [FromBody] SchedulingStatusUpdateRequestDto request)
    {
        OperationResult<SchedulingResponseDto> result = _schedulingService.UpdateStatus(id, request);

        return StatusCode((int)result.StatusCode, result);
    }
}
