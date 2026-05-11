using cabeleleira_leila.Enums;

namespace cabeleleira_leila.DTO;

public class ClienteUpdateRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Status Status { get; set; }
    public UserRole Role { get; set; } = UserRole.Cliente;
}
