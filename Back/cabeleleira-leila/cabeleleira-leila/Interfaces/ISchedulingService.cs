using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface ISchedulingService : IBaseService<SchedulingRequestDto, SchedulingUpdateRequestDto, SchedulingResponseDto, long>
{
    Task<OperationResult<List<SchedulingResponseDto>>> GetByClienteAsync(long clienteId, DateTime? start, DateTime? end, CancellationToken cancellationToken = default);
    Task<OperationResult<SchedulingResponseDto>> GetSameWeekSuggestionAsync(long clienteId, DateTime dataHora, CancellationToken cancellationToken = default);
    Task<OperationResult<SchedulingResponseDto>> CreateAsync(SchedulingRequestDto request, CancellationToken cancellationToken = default);
    Task<OperationResult<SchedulingResponseDto>> UpdateAsync(long id, SchedulingUpdateRequestDto request, CancellationToken cancellationToken = default);
    Task<OperationResult<SchedulingResponseDto>> AdminUpdateAsync(long id, SchedulingUpdateRequestDto request, CancellationToken cancellationToken = default);
    Task<OperationResult<SchedulingResponseDto>> UpdateStatusAsync(long id, SchedulingStatusUpdateRequestDto request, CancellationToken cancellationToken = default);
}
