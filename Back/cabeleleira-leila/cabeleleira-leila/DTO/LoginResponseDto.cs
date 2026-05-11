using cabeleleira_leila.Enums;

namespace cabeleleira_leila.DTO;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserRole Role { get; set; } = UserRole.Cliente;
    public ClienteResponseDto Cliente { get; set; } = new();
}
