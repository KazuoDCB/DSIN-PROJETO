using cabeleleira_leila.DTO;
using cabeleleira_leila.Enums;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using MapsterMapper;
using System.Text.RegularExpressions;

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

    public OperationResult<ServicoResponseDto> Create(ServicoRequestDto request)
    {
        var errors = Validate(request.Name, request.Description, request.Price, request.Duration, request.Status);
        if (errors.Count > 0) return OperationResult<ServicoResponseDto>.UnprocessableEntity(errors);

        var name = NormalizeText(request.Name);
        var description = NormalizeText(request.Description);

        if (_servicoRepository.NameExists(name, null)) return OperationResult<ServicoResponseDto>.UnprocessableEntity(Error("Name", "Ja existe um servico cadastrado com este nome."));

        var servico = new Servico(name, description, request.Price, request.Duration, request.Status);

        _servicoRepository.Create(servico);

        return OperationResult<ServicoResponseDto>.Created(Mapper.Map<ServicoResponseDto>(servico));
    }

    public OperationResult<ServicoResponseDto> Update(
        long id,
        ServicoUpdateRequestDto request)
    {
        var errors = Validate(request.Name, request.Description, request.Price, request.Duration, request.Status);
        if (errors.Count > 0) return OperationResult<ServicoResponseDto>.UnprocessableEntity(errors);

        var servico = _servicoRepository.GetById(id);

        if (servico is null) return OperationResult<ServicoResponseDto>.NotFound(NotFoundError());

        var name = NormalizeText(request.Name);
        var description = NormalizeText(request.Description);

        if (_servicoRepository.NameExists(name, id)) return OperationResult<ServicoResponseDto>.UnprocessableEntity(Error("Name", "Ja existe outro servico cadastrado com este nome."));

        servico.Update(name, description, request.Price, request.Duration, request.Status);

        _servicoRepository.Update(servico);

        return OperationResult<ServicoResponseDto>.Ok(Mapper.Map<ServicoResponseDto>(servico));
    }

    private static List<ErrorMessage> Validate(string name, string description, decimal price, int duration, Status status)
    {
        var errors = new List<ErrorMessage>();
        var normalizedName = NormalizeText(name);
        var normalizedDescription = NormalizeText(description);

        if (normalizedName.Length < 3) errors.Add(Error("Name", "Informe um nome com pelo menos 3 caracteres."));
        if (normalizedName.Length > 120) errors.Add(Error("Name", "O nome deve ter no maximo 120 caracteres."));
        if (normalizedDescription.Length < 10) errors.Add(Error("Description", "Informe uma descricao com pelo menos 10 caracteres."));
        if (normalizedDescription.Length > 500) errors.Add(Error("Description", "A descricao deve ter no maximo 500 caracteres."));
        if (price <= 0) errors.Add(Error("Price", "O preco do servico deve ser maior que zero."));
        if (duration < 1 || duration > 1440) errors.Add(Error("Duration", "A duracao deve estar entre 1 e 1440 minutos."));
        if (!Enum.IsDefined(status)) errors.Add(Error("Status", "Status de servico invalido."));

        return errors;
    }

    private static string NormalizeText(string value)
    {
        return Regex.Replace(value?.Trim() ?? string.Empty, "\\s+", " ");
    }
}
