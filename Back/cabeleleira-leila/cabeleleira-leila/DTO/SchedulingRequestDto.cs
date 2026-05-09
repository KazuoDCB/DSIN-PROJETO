using System.ComponentModel.DataAnnotations;

namespace cabeleleira_leila.DTO;

public class SchedulingRequestDto
{
    [Range(1, long.MaxValue)]
    public long ClienteId { get; set; }

    public DateTime DataHora { get; set; }

    [MinLength(1)]
    public List<long> ServicoIds { get; set; } = [];
}
