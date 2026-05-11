using cabeleleira_leila.Enums;

namespace cabeleleira_leila.DTO;

public class ServicoUpdateRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Duration { get; set; }

    public Status Status { get; set; }
}
