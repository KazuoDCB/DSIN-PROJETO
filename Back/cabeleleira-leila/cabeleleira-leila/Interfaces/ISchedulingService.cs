using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface ISchedulingService : IBaseService<SchedulingRequestDto, SchedulingUpdateRequestDto, SchedulingResponseDto, long>
{
    OperationResult<List<SchedulingResponseDto>> GetByCliente(long clienteId, DateTime? start, DateTime? end);
    OperationResult<SchedulingResponseDto> GetSameWeekSuggestion(long clienteId, DateTime dataHora);
    OperationResult<SchedulingResponseDto> Create(SchedulingRequestDto request);
    OperationResult<SchedulingResponseDto> Update(long id, SchedulingUpdateRequestDto request);
    OperationResult<SchedulingResponseDto> AdminUpdate(long id, SchedulingUpdateRequestDto request);
    OperationResult<SchedulingResponseDto> UpdateStatus(long id, SchedulingStatusUpdateRequestDto request);
}
