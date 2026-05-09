using cabeleleira_leila.DTO;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using MapsterMapper;

namespace cabeleleira_leila.Services;

public class SchedulingService :
    BaseService<Scheduling, SchedulingRequestDto, SchedulingUpdateRequestDto, SchedulingResponseDto, long>,
    ISchedulingService
{
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

    public override async Task<OperationResult<List<SchedulingResponseDto>>> GetAllAsync(
        int page,
        int size,
        CancellationToken cancellationToken = default)
    {
        var schedulings = await _schedulingRepository.GetAllWithDetailsAsync(page, size, cancellationToken);

        return OperationResult<List<SchedulingResponseDto>>.Ok(Mapper.Map<List<SchedulingResponseDto>>(schedulings));
    }

    public async Task<OperationResult<List<SchedulingResponseDto>>> GetByClienteAsync(
        long clienteId,
        DateTime? start,
        DateTime? end,
        CancellationToken cancellationToken = default)
    {
        if (!await _clienteRepository.ExistsAsync(clienteId, cancellationToken)) return OperationResult<List<SchedulingResponseDto>>.NotFound(Error("ClienteId", "Cliente nao encontrado."));

        var schedulings = await _schedulingRepository.GetByClienteWithDetailsAsync(clienteId, start, end, cancellationToken);

        return OperationResult<List<SchedulingResponseDto>>.Ok(Mapper.Map<List<SchedulingResponseDto>>(schedulings));
    }

    public async Task<OperationResult<SchedulingResponseDto>> GetSameWeekSuggestionAsync(
        long clienteId,
        DateTime dataHora,
        CancellationToken cancellationToken = default)
    {
        if (!await _clienteRepository.ExistsAsync(clienteId, cancellationToken)) return OperationResult<SchedulingResponseDto>.NotFound(Error("ClienteId", "Cliente nao encontrado."));

        var scheduling = await _schedulingRepository.GetSameWeekAsync(clienteId, dataHora, cancellationToken);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(Error("DataHora", "Nenhum agendamento encontrado para este cliente na mesma semana."));

        return OperationResult<SchedulingResponseDto>.Ok(Mapper.Map<SchedulingResponseDto>(scheduling));
    }

    public override async Task<OperationResult<SchedulingResponseDto>> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var scheduling = await _schedulingRepository.GetWithDetailsAsync(id, true, cancellationToken);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(NotFoundError());

        return OperationResult<SchedulingResponseDto>.Ok(Mapper.Map<SchedulingResponseDto>(scheduling));
    }

    public async Task<OperationResult<SchedulingResponseDto>> CreateAsync(
        SchedulingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!await _clienteRepository.ExistsAsync(request.ClienteId, cancellationToken)) return OperationResult<SchedulingResponseDto>.NotFound(Error("ClienteId", "Cliente nao encontrado."));
        if (await _schedulingRepository.HasSchedulingAtAsync(request.DataHora, null, cancellationToken)) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("DataHora", "Ja existe um agendamento neste horario."));

        var servicosResult = await GetServicosAsync(request.ServicoIds, cancellationToken);

        if (!servicosResult.Success) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(servicosResult.Errors);

        var scheduling = new Scheduling(request.ClienteId, request.DataHora, servicosResult.Data!);

        await _schedulingRepository.CreateAsync(scheduling, cancellationToken);

        return await CreatedResponseAsync(scheduling.Id, cancellationToken);
    }

    public async Task<OperationResult<SchedulingResponseDto>> UpdateAsync(
        long id,
        SchedulingUpdateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var scheduling = await _schedulingRepository.GetWithDetailsAsync(id, false, cancellationToken);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(NotFoundError());
        if (scheduling.DataHora <= DateTime.Now.AddDays(2)) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("DataHora", "Alteracoes so podem ser realizadas por telefone."));

        return await ApplyUpdateAsync(scheduling, request, cancellationToken);
    }

    public async Task<OperationResult<SchedulingResponseDto>> AdminUpdateAsync(
        long id,
        SchedulingUpdateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var scheduling = await _schedulingRepository.GetWithDetailsAsync(id, false, cancellationToken);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(NotFoundError());

        return await ApplyUpdateAsync(scheduling, request, cancellationToken);
    }

    public async Task<OperationResult<SchedulingResponseDto>> UpdateStatusAsync(
        long id,
        SchedulingStatusUpdateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var scheduling = await _schedulingRepository.GetWithDetailsAsync(id, false, cancellationToken);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.NotFound(NotFoundError());

        scheduling.UpdateStatus(request.Status);

        await _schedulingRepository.UpdateAsync(scheduling, cancellationToken);

        return OperationResult<SchedulingResponseDto>.Ok(Mapper.Map<SchedulingResponseDto>(scheduling));
    }

    private async Task<OperationResult<SchedulingResponseDto>> ApplyUpdateAsync(
        Scheduling scheduling,
        SchedulingUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        if (await _schedulingRepository.HasSchedulingAtAsync(request.DataHora, scheduling.Id, cancellationToken)) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(Error("DataHora", "Ja existe um agendamento neste horario."));

        var servicosResult = await GetServicosAsync(request.ServicoIds, cancellationToken);

        if (!servicosResult.Success) return OperationResult<SchedulingResponseDto>.UnprocessableEntity(servicosResult.Errors);

        scheduling.Update(request.DataHora, request.Status, servicosResult.Data!);

        await _schedulingRepository.UpdateAsync(scheduling, cancellationToken);

        return OperationResult<SchedulingResponseDto>.Ok(Mapper.Map<SchedulingResponseDto>(scheduling));
    }

    private async Task<OperationResult<List<Servico>>> GetServicosAsync(
        List<long> servicoIds,
        CancellationToken cancellationToken)
    {
        var uniqueIds = servicoIds
            .Distinct()
            .ToList();

        if (uniqueIds.Count is 0) return OperationResult<List<Servico>>.UnprocessableEntity(Error("ServicoIds", "Informe ao menos um servico."));

        var servicos = await _servicoRepository.GetByIdsAsync(uniqueIds, cancellationToken);

        if (servicos.Count != uniqueIds.Count) return OperationResult<List<Servico>>.UnprocessableEntity(Error("ServicoIds", "Um ou mais servicos informados nao foram encontrados."));

        return OperationResult<List<Servico>>.Ok(servicos);
    }

    private async Task<OperationResult<SchedulingResponseDto>> CreatedResponseAsync(
        long schedulingId,
        CancellationToken cancellationToken)
    {
        var scheduling = await _schedulingRepository.GetWithDetailsAsync(schedulingId, true, cancellationToken);

        if (scheduling is null) return OperationResult<SchedulingResponseDto>.FatalError(Error("Agendamento", "Agendamento criado, mas nao foi possivel carregar o retorno."));

        return OperationResult<SchedulingResponseDto>.Created(Mapper.Map<SchedulingResponseDto>(scheduling));
    }
}
