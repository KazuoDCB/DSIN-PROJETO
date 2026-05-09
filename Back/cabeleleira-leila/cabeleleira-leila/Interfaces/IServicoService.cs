using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IServicoService : IBaseService<ServicoRequestDto, ServicoUpdateRequestDto, ServicoResponseDto, long>
{
    Task<OperationResult<ServicoResponseDto>> CreateAsync(ServicoRequestDto request, CancellationToken cancellationToken = default);
    Task<OperationResult<ServicoResponseDto>> UpdateAsync(long id, ServicoUpdateRequestDto request, CancellationToken cancellationToken = default);
}
