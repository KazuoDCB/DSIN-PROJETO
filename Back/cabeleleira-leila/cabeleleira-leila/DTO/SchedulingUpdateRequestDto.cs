using cabeleleira_leila.Enums;

namespace cabeleleira_leila.DTO;

public class SchedulingUpdateRequestDto
{
    public DateTime DataHora { get; set; }
    public StatusAgendamento Status { get; set; }

    public List<long> ServicoIds { get; set; } = [];
}
