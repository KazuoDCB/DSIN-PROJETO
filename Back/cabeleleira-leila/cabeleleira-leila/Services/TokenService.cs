using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cabeleleira_leila.DTO;
using cabeleleira_leila.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace cabeleleira_leila.Services;

public class TokenService : ITokenService
{
    private readonly string _secretKey;

    public TokenService(IConfiguration configuration)
    {
        _secretKey = configuration.GetSection("AppSettings")["SecretKey"]
            ?? throw new InvalidOperationException("Configuracao obrigatoria nao encontrada: AppSettings:SecretKey.");
    }

    public string Generate(ClienteResponseDto cliente)
    {
        JwtSecurityTokenHandler handler = new();
        byte[] key = Encoding.ASCII.GetBytes(_secretKey);

        SigningCredentials credentials = new(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = GenerateClaims(cliente),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2)
        };

        SecurityToken token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(ClienteResponseDto cliente)
    {
        ClaimsIdentity claimsIdentity = new();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, cliente.Name));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, cliente.Email));

        return claimsIdentity;
    }
}
