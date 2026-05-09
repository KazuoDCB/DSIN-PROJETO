using System.ComponentModel.DataAnnotations;
using cabeleleira_leila.Enums;

namespace cabeleleira_leila.DTO;

public class ClienteUpdateRequestDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    public Status Status { get; set; }
}
