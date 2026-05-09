using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IClienteService : IBaseService<ClienteRequestDto, ClienteUpdateRequestDto, ClienteResponseDto, long>
{
    Task<OperationResult<ClienteResponseDto>> CreateAsync(ClienteRequestDto request, CancellationToken cancellationToken = default);
    Task<OperationResult<ClienteResponseDto>> UpdateAsync(long id, ClienteUpdateRequestDto request, CancellationToken cancellationToken = default);
}
