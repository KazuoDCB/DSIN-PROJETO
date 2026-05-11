using cabeleleira_leila.DTO;
using cabeleleira_leila.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cabeleleira_leila.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        var result = _authService.Login(request);

        return this.ToActionResult(result);
    }

    [AllowAnonymous]
    [HttpPost("admin/login")]
    public IActionResult AdminLogin([FromBody] LoginRequestDto request)
    {
        var result = _authService.AdminLogin(request);

        return this.ToActionResult(result);
    }
}
