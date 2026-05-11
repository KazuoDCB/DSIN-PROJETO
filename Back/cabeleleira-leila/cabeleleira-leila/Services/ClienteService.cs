using cabeleleira_leila.DTO;
using cabeleleira_leila.Enums;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using MapsterMapper;
using System.Text.RegularExpressions;

namespace cabeleleira_leila.Services;

public class ClienteService :
    BaseService<User, ClienteRequestDto, ClienteUpdateRequestDto, ClienteResponseDto, long>,
    IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IPasswordHashService _passwordHashService;

    public ClienteService(
        IClienteRepository clienteRepository,
        IPasswordHashService passwordHashService,
        IMapper mapper) : base(clienteRepository, mapper)
    {
        _clienteRepository = clienteRepository;
        _passwordHashService = passwordHashService;
    }

    public OperationResult<ClienteResponseDto> Create(ClienteRequestDto request)
    {
        var errors = ValidateCreate(request);
        if (errors.Count > 0) return OperationResult<ClienteResponseDto>.UnprocessableEntity(errors);

        var name = NormalizeText(request.Name);
        var number = NormalizePhone(request.Number);
        var email = request.Email.Trim().ToLowerInvariant();

        if (_clienteRepository.EmailExists(email, null)) return OperationResult<ClienteResponseDto>.UnprocessableEntity(Error("Email", "Indisponivel"));

        var passwordHash = _passwordHashService.Hash(request.Password);
        var user = new User(name, number, email, passwordHash, UserRole.Cliente);

        _clienteRepository.Create(user);

        return OperationResult<ClienteResponseDto>.Created(Mapper.Map<ClienteResponseDto>(user));
    }

    public OperationResult<ClienteResponseDto> Update(
        long id,
        ClienteUpdateRequestDto request)
    {
        var errors = ValidateUpdate(request);
        if (errors.Count > 0) return OperationResult<ClienteResponseDto>.UnprocessableEntity(errors);

        var user = _clienteRepository.GetById(id);

        if (user is null) return OperationResult<ClienteResponseDto>.NotFound(NotFoundError());

        var name = NormalizeText(request.Name);
        var number = NormalizePhone(request.Number);
        var email = request.Email.Trim().ToLowerInvariant();

        if (_clienteRepository.EmailExists(email, id)) return OperationResult<ClienteResponseDto>.UnprocessableEntity(Error("Email", "Indisponivel."));

        user.Update(name, number, email, request.Status, request.Role);

        _clienteRepository.Update(user);

        return OperationResult<ClienteResponseDto>.Ok(Mapper.Map<ClienteResponseDto>(user));
    }

    private static List<ErrorMessage> ValidateCreate(ClienteRequestDto request)
    {
        var errors = ValidateBase(request.Name, request.Number, request.Email);

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            errors.Add(Error("Password", "A senha deve ter pelo menos 8 caracteres."));
        }
        else if (!Regex.IsMatch(request.Password, "[A-Za-z]") || !Regex.IsMatch(request.Password, "\\d"))
        {
            errors.Add(Error("Password", "A senha deve conter letras e numeros."));
        }

        return errors;
    }

    private static List<ErrorMessage> ValidateUpdate(ClienteUpdateRequestDto request)
    {
        var errors = ValidateBase(request.Name, request.Number, request.Email);

        if (!Enum.IsDefined(request.Status)) errors.Add(Error("Status", "Status de cliente invalido."));
        if (!Enum.IsDefined(request.Role)) errors.Add(Error("Role", "Perfil de usuario invalido."));

        return errors;
    }

    private static List<ErrorMessage> ValidateBase(string name, string number, string email)
    {
        var errors = new List<ErrorMessage>();
        var normalizedName = NormalizeText(name);
        var phoneDigits = NormalizePhone(number);
        var normalizedEmail = email?.Trim() ?? string.Empty;

        if (normalizedName.Length < 3) errors.Add(Error("Name", "Informe um nome com pelo menos 3 caracteres."));
        if (normalizedName.Length > 150) errors.Add(Error("Name", "O nome deve ter no maximo 150 caracteres."));
        if (!Regex.IsMatch(phoneDigits, "^\\d{10,11}$")) errors.Add(Error("Number", "Informe um telefone valido com DDD."));
        if (normalizedEmail.Length > 255 || !Regex.IsMatch(normalizedEmail, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")) errors.Add(Error("Email", "Informe um email valido."));

        return errors;
    }

    private static string NormalizeText(string value)
    {
        return Regex.Replace(value?.Trim() ?? string.Empty, "\\s+", " ");
    }

    private static string NormalizePhone(string value)
    {
        return Regex.Replace(value ?? string.Empty, "\\D", string.Empty);
    }
}
