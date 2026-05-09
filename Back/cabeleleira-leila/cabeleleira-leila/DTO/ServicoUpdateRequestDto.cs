using System.ComponentModel.DataAnnotations;
using cabeleleira_leila.Enums;

namespace cabeleleira_leila.DTO;

public class ServicoUpdateRequestDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    [Range(1, 1440)]
    public int Duration { get; set; }

    public Status Status { get; set; }
}
