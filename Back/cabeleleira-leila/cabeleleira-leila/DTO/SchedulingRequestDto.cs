namespace cabeleleira_leila.DTO;

public class SchedulingRequestDto
{
    public long ClienteId { get; set; }

    public DateTime DataHora { get; set; }

    public List<long> ServicoIds { get; set; } = [];
}
