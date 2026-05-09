using System.Text;
using cabeleleira_leila.DataBase;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Mappings;
using cabeleleira_leila.Repositories;
using cabeleleira_leila.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Cabeleleira_LeilaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var secretKey = builder.Configuration.GetSection("AppSettings")["SecretKey"]
    ?? throw new InvalidOperationException("Configuracao obrigatoria nao encontrada: AppSettings:SecretKey.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<ISchedulingRepository, SchedulingRepository>();
builder.Services.AddScoped<IPasswordHashService, Argon2IdPasswordHashService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IServicoService, ServicoService>();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();
builder.Services.AddServiceMapper();

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
