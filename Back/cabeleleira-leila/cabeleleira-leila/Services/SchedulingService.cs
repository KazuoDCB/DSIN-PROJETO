using cabeleleira_leila.DTO;
using cabeleleira_leila.Enums;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using MapsterMapper;

namespace cabeleleira_leila.Services;

public class SchedulingService :
    BaseService<Scheduling, SchedulingRequestDto, SchedulingUpdateRequestDto, SchedulingResponseDto, long>,
    ISchedulingService
{
    private static readonly TimeZoneInfo SaoPauloTimeZone = ResolveSaoPauloTimeZone();
    private readonly ISchedulingRepository _schedulingRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IServicoRepository _servicoRepository;

    public SchedulingService(
        ISchedulingRepository schedulingRepository,
        IClienteRepository clienteRepository,
        IServicoRepository servicoRepository,
        IMapper mapper) : base(schedulingRepository, mapper)
    {
        _schedulingRepository = schedulingRepository;
        _clienteRepository = clienteRepository;
        _servicoRepository = servicoRepository;
    }

    public override OperationResult<List<SchedulingResponseDto>> GetAll(
        int page,
        int size)
    {
        var schedulings = _schedulingRepository.GetAllWithDetails(page, size);

        return OperationResult<List<SchedulingResponseDto>>.Ok(Mapper.Map<List<SchedulingResponseDto>>(schedulings));
    }

    public OperationResult<List<SchedulingResponseDto>> GetByCliente(
        long clienteId,
        DateTime? start,
        DateTime? end)
    {
        if (!_clienteRepository.Exists(clienteId)) return OperationResult<List<SchedulingResponseDto>>.NotFound(Error("ClienteId", "Cliente nao encontrado."));
        if (start.HasValue && end.HasValue && NormalizeToUtc(start.Value) > NormalizeToUtc(end.Value)) return OperationResult<List<SchedulingResponseDto>>.UnprocessableEntity(Error("Periodo", "A data inicial deve ser anterior a data final."));

        var schedulings = _schedulingRepository.GetByClienteWithDetails(
            clienteId,
            start.HasValue ? NormalizeToUtc(start.Value) : null,
            end.HasValue ? NormalizeToUtc(end.Value) : null);

        return OperationResult<List<SchedulingResponseDto>>.Ok(Mapper.Map<List<SchedulingResponseDto>>(schedulings));
    }

    public OperationResult<SchedulingResponseDto> GetSameWeekSuggestion(
        long clienteId,
        DateTime dataHora)
    {
        if (!_clienteRepository.Exists(clienteId)) return OperationResult<SchedulingResponseDto>.NotFound(Error("ClienteId", "Cliente nao encontrado."));

        var normalizedDataHora = NormalizeToUtc(dataHora);
        var scheduling = _schedulingRepository.GetSameWeek(clienteId, normalizedDataHora);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(Error("DataHora", "Nenhum agendamento encontrado para este cliente na mesma semana."));

        return OperationResult<SchedulingResponseDto>.Ok(Mapper.Map<SchedulingResponseDto>(scheduling));
    }

    public override OperationResult<SchedulingResponseDto> GetById(long id)
    {
        var scheduling = _schedulingRepository.GetWithDetails(id, true);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(NotFoundError());

        return OperationResult<SchedulingResponseDto>.Ok(Mapper.Map<SchedulingResponseDto>(scheduling));
    }

    public OperationResult<SchedulingResponseDto> Create(SchedulingRequestDto request)
    {
        var normalizedDataHora = NormalizeToUtc(request.DataHora);
        var errors = ValidateRequest(normalizedDataHora, request.ServicoIds, true);

        if (errors.Count > 0) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(errors);

        var cliente = _clienteRepository.GetById(request.ClienteId);
        if (cliente is null) return OperationResult<SchedulingResponseDto>.NotFound(Error("ClienteId", "Cliente nao encontrado."));
        if (cliente.Status is not Status.Ativo) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("ClienteId", "Cliente inativo nao pode realizar agendamentos."));
        if (cliente.Role is not UserRole.Cliente) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("ClienteId", "Informe um usuario cliente para o agendamento."));
        if (_schedulingRepository.HasSchedulingAt(normalizedDataHora, null)) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("DataHora", "Ja existe um agendamento neste horario."));

        var servicosResult = GetServicos(request.ServicoIds);

        if (!servicosResult.Success) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(servicosResult.Errors);

        var scheduling = new Scheduling(request.ClienteId, normalizedDataHora, servicosResult.Data!);

        _schedulingRepository.Create(scheduling);

        return CreatedResponse(scheduling.Id);
    }

    public OperationResult<SchedulingResponseDto> Update(
        long id,
        SchedulingUpdateRequestDto request)
    {
        var scheduling = _schedulingRepository.GetWithDetails(id, false);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(NotFoundError());
        if (scheduling.DataHora <= DateTime.UtcNow.AddDays(2)) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("DataHora", "Alteracoes so podem ser realizadas por telefone."));

        return ApplyUpdate(scheduling, request, false);
    }

    public OperationResult<SchedulingResponseDto> AdminUpdate(
        long id,
        SchedulingUpdateRequestDto request)
    {
        var scheduling = _schedulingRepository.GetWithDetails(id, false);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(NotFoundError());

        return ApplyUpdate(scheduling, request, true);
    }

    public OperationResult<SchedulingResponseDto> UpdateStatus(
        long id,
        SchedulingStatusUpdateRequestDto request)
    {
        if (!Enum.IsDefined(request.Status)) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("Status", "Status de agendamento invalido."));

        var scheduling = _schedulingRepository.GetWithDetails(id, false);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(NotFoundError());

        scheduling.UpdateStatus(request.Status);

        _schedulingRepository.Update(scheduling);

        return OperationResult<SchedulingResponseDto>.Ok(Mapper.Map<SchedulingResponseDto>(scheduling));
    }

    public override OperationResult Delete(long id)
    {
        var scheduling = _schedulingRepository.GetById(id);

        if (scheduling is null) return OperationResult.NotFound(NotFoundError());
        if (scheduling.DataHora <= DateTime.UtcNow.AddDays(2)) return OperationResult.UnprocessableEntity(Error("DataHora", "Alteracoes so podem ser realizadas por telefone."));

        _schedulingRepository.Delete(scheduling);

        return OperationResult.Ok();
    }

    private OperationResult<SchedulingResponseDto> ApplyUpdate(
        Scheduling scheduling,
        SchedulingUpdateRequestDto request,
        bool allowStatusChange)
    {
        var normalizedDataHora = NormalizeToUtc(request.DataHora);
        var currentDataHora = NormalizeToUtc(scheduling.DataHora);
        var dataHoraChanged = normalizedDataHora != currentDataHora;
        var errors = ValidateRequest(normalizedDataHora, request.ServicoIds, dataHoraChanged);

        if (allowStatusChange && !Enum.IsDefined(request.Status)) errors.Add(Error("Status", "Status de agendamento invalido."));
        if (errors.Count > 0) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(errors);

        if (_schedulingRepository.HasSchedulingAt(normalizedDataHora, scheduling.Id)) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("DataHora", "Ja existe um agendamento neste horario."));

        var servicosResult = GetServicos(request.ServicoIds);

        if (!servicosResult.Success) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(servicosResult.Errors);

        scheduling.Update(normalizedDataHora, allowStatusChange ? request.Status : scheduling.Status, servicosResult.Data!);

        _schedulingRepository.Update(scheduling);

        return OperationResult<SchedulingResponseDto>.Ok(Mapper.Map<SchedulingResponseDto>(scheduling));
    }

    private OperationResult<List<Servico>> GetServicos(List<long> servicoIds)
    {
        var uniqueIds = servicoIds
            .Distinct()
            .ToList();

        if (uniqueIds.Count is 0) return OperationResult<List<Servico>>.UnprocessableEntity(Error("ServicoIds", "Informe ao menos um servico."));

        var servicos = _servicoRepository.GetByIds(uniqueIds);

        if (servicos.Count != uniqueIds.Count) return OperationResult<List<Servico>>.UnprocessableEntity(Error("ServicoIds", "Um ou mais servicos informados nao foram encontrados."));
        if (servicos.Any(servico => servico.Status is not Status.Ativo)) return OperationResult<List<Servico>>.UnprocessableEntity(Error("ServicoIds", "Informe apenas servicos ativos."));

        return OperationResult<List<Servico>>.Ok(servicos);
    }

    private static List<ErrorMessage> ValidateRequest(DateTime dataHora, List<long>? servicoIds, bool requireFutureDate)
    {
        var errors = new List<ErrorMessage>();

        if (dataHora == default) errors.Add(Error("DataHora", "Informe a data e hora do agendamento."));
        if (requireFutureDate && dataHora <= DateTime.UtcNow) errors.Add(Error("DataHora", "O agendamento deve ser feito para uma data futura."));
        if (servicoIds is null || servicoIds.Count == 0) errors.Add(Error("ServicoIds", "Informe ao menos um servico."));
        if (servicoIds?.Any(id => id <= 0) == true) errors.Add(Error("ServicoIds", "Informe apenas servicos validos."));

        return errors;
    }

    private static DateTime NormalizeToUtc(DateTime dateTime)
    {
        if (dateTime == default) return default;

        return dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            _ => TimeZoneInfo.ConvertTimeToUtc(dateTime, SaoPauloTimeZone)
        };
    }

    private static TimeZoneInfo ResolveSaoPauloTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }
    }

    private OperationResult<SchedulingResponseDto> CreatedResponse(long schedulingId)
    {
        var scheduling = _schedulingRepository.GetWithDetails(schedulingId, true);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.FatalError(Error("Agendamento", "Agendamento criado, mas nao foi possivel carregar o retorno."));

        return OperationResult<SchedulingResponseDto>.Created(Mapper.Map<SchedulingResponseDto>(scheduling));
    }
}
