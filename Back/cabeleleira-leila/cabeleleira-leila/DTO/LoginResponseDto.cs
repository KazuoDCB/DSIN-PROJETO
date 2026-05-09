namespace cabeleleira_leila.DTO;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public ClienteResponseDto Cliente { get; set; } = new();
}
