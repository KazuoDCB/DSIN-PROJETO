using cabeleleira_leila.Enums;

namespace cabeleleira_leila.DTO;

public class SchedulingResponseDto
{
    public long Id { get; set; }
    public long ClienteId { get; set; }
    public string ClienteName { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public StatusAgendamento Status { get; set; }
    public decimal TotalValue { get; set; }
    public int TotalDuration { get; set; }
    public IReadOnlyList<ServicoResponseDto> Servicos { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
