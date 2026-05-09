using cabeleleira_leila.DTO;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using MapsterMapper;

namespace cabeleleira_leila.Services;

public class ClienteService :
    BaseService<Cliente, ClienteRequestDto, ClienteUpdateRequestDto, ClienteResponseDto, long>,
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

    public async Task<OperationResult<ClienteResponseDto>> CreateAsync(
        ClienteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (await _clienteRepository.EmailExistsAsync(request.Email, null, cancellationToken)) return OperationResult<ClienteResponseDto>.UnprocessableEntity(Error("Email", "Ja existe um cliente cadastrado com este email."));

        var passwordHash = _passwordHashService.Hash(request.Password);
        var cliente = new Cliente(request.Name, request.Number, request.Email, passwordHash);

        await _clienteRepository.CreateAsync(cliente, cancellationToken);

        return OperationResult<ClienteResponseDto>.Created(Mapper.Map<ClienteResponseDto>(cliente));
    }

    public async Task<OperationResult<ClienteResponseDto>> UpdateAsync(
        long id,
        ClienteUpdateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);

        if (cliente is null) return OperationResult<ClienteResponseDto>.NotFound(NotFoundError());
        if (await _clienteRepository.EmailExistsAsync(request.Email, id, cancellationToken)) return OperationResult<ClienteResponseDto>.UnprocessableEntity(Error("Email", "Ja existe outro cliente cadastrado com este email."));

        cliente.Update(request.Name, request.Number, request.Email, request.Status);

        await _clienteRepository.UpdateAsync(cliente, cancellationToken);

        return OperationResult<ClienteResponseDto>.Ok(Mapper.Map<ClienteResponseDto>(cliente));
    }
}
