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
        return LoginByRole(request, UserRole.Cliente);
    }

    public OperationResult<LoginResponseDto> AdminLogin(LoginRequestDto request)
    {
        return LoginByRole(request, UserRole.Admin);
    }

    private OperationResult<LoginResponseDto> LoginByRole(LoginRequestDto request, UserRole expectedRole)
    {
        var user = _clienteRepository.GetByEmail(request.Email);

        if (user is null) return Unauthorized();
        if (!_passwordHashService.Verify(request.Password, user.PasswordHash)) return Unauthorized();
        if (user.Status is Status.Inativo) return OperationResult<LoginResponseDto>.Forbidden(Error("User", "Usuario inativo."));
        if (user.Role != expectedRole) return OperationResult<LoginResponseDto>.Forbidden(Error("Role", "Usuario nao autorizado para este tipo de login."));

        var userResponse = _mapper.Map<ClienteResponseDto>(user);
        var response = new LoginResponseDto
        {
            AccessToken = _tokenService.Generate(userResponse),
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            Role = userResponse.Role,
            Cliente = userResponse
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
