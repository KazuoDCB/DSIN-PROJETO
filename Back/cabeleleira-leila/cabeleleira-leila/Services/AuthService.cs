using cabeleleira_leila.DTO;
using cabeleleira_leila.Enums;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using MapsterMapper;

namespace cabeleleira_leila.Services;

public class AuthService : IAuthService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        IClienteRepository clienteRepository,
        IPasswordHashService passwordHashService,
        ITokenService tokenService,
        IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _passwordHashService = passwordHashService;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public OperationResult<LoginResponseDto> Login(LoginRequestDto request)
    {
        var cliente = _clienteRepository.GetByEmail(request.Email);

        if (cliente is null) return Unauthorized();
        if (!_passwordHashService.Verify(request.Password, cliente.PasswordHash)) return Unauthorized();
        if (cliente.Status is Status.Inativo) return OperationResult<LoginResponseDto>.Forbidden(Error("Cliente", "Cliente inativo."));

        var clienteResponse = _mapper.Map<ClienteResponseDto>(cliente);
        var response = new LoginResponseDto
        {
            AccessToken = _tokenService.Generate(clienteResponse),
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            Cliente = clienteResponse
        };

        return OperationResult<LoginResponseDto>.Ok(response);
    }

    private static OperationResult<LoginResponseDto> Unauthorized()
    {
        return OperationResult<LoginResponseDto>.Unauthorized(Error("Login", "Email ou senha inválidos."));
    }

    private static ErrorMessage Error(string property, string message)
    {
        return ErrorMessage.CreateErrorMessage(property, message);
    }
}
