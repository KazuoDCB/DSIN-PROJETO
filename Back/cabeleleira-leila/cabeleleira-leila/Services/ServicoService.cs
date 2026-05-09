using cabeleleira_leila.DTO;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using MapsterMapper;

namespace cabeleleira_leila.Services;

public class ServicoService :
    BaseService<Servico, ServicoRequestDto, ServicoUpdateRequestDto, ServicoResponseDto, long>,
    IServicoService
{
    private readonly IServicoRepository _servicoRepository;

    public ServicoService(IServicoRepository servicoRepository, IMapper mapper) : base(servicoRepository, mapper)
    {
        _servicoRepository = servicoRepository;
    }

    public async Task<OperationResult<ServicoResponseDto>> CreateAsync(
        ServicoRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (await _servicoRepository.NameExistsAsync(request.Name, null, cancellationToken)) return OperationResult<ServicoResponseDto>.UnprocessableEntity(Error("Name", "Ja existe um servico cadastrado com este nome."));

        var servico = new Servico(request.Name, request.Price, request.Duration, request.Status);

        await _servicoRepository.CreateAsync(servico, cancellationToken);

        return OperationResult<ServicoResponseDto>.Created(Mapper.Map<ServicoResponseDto>(servico));
    }

    public async Task<OperationResult<ServicoResponseDto>> UpdateAsync(
        long id,
        ServicoUpdateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var servico = await _servicoRepository.GetByIdAsync(id, cancellationToken);

        if (servico is null) return OperationResult<ServicoResponseDto>.NotFound(NotFoundError());
        if (await _servicoRepository.NameExistsAsync(request.Name, id, cancellationToken)) return OperationResult<ServicoResponseDto>.UnprocessableEntity(Error("Name", "Ja existe outro servico cadastrado com este nome."));

        servico.Update(request.Name, request.Price, request.Duration, request.Status);

        await _servicoRepository.UpdateAsync(servico, cancellationToken);

        return OperationResult<ServicoResponseDto>.Ok(Mapper.Map<ServicoResponseDto>(servico));
    }
}
