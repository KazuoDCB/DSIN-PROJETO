using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IClienteService : IBaseService<ClienteRequestDto, ClienteUpdateRequestDto, ClienteResponseDto, long>
{
    OperationResult<ClienteResponseDto> Create(ClienteRequestDto request);
    OperationResult<ClienteResponseDto> Update(long id, ClienteUpdateRequestDto request);
}
