using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IServicoService : IBaseService<ServicoRequestDto, ServicoUpdateRequestDto, ServicoResponseDto, long>
{
    OperationResult<ServicoResponseDto> Create(ServicoRequestDto request);
    OperationResult<ServicoResponseDto> Update(long id, ServicoUpdateRequestDto request);
}
