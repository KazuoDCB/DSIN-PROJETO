using cabeleleira_leila.DTO;

namespace cabeleleira_leila.Interfaces;

public interface ITokenService
{
    string Generate(ClienteResponseDto cliente);
}
