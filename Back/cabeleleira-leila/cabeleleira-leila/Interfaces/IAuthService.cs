using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IAuthService
{
    OperationResult<LoginResponseDto> Login(LoginRequestDto request);
}
