using System.Text;
using cabeleleira_leila.DataBase;
using cabeleleira_leila.Enums;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Mappings;
using cabeleleira_leila.Models;
using cabeleleira_leila.Repositories;
using cabeleleira_leila.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddServiceMapper();

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .SelectMany(modelState => modelState.Value?.Errors.Select(_ =>
                    ErrorMessage.CreateErrorMessage(
                        modelState.Key,
                        "Informe um valor valido.")) ?? [])
                .ToList();

            var result = OperationResult.UnprocessableEntity(errors);
            return new ObjectResult(result)
            {
                StatusCode = (int)result.StatusCode
            };
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowSpecificOrigin");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

SeedAdminUser(app);

app.Run();

static void SeedAdminUser(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeed");
    var email = configuration["AdminCredentials:Email"];
    var password = configuration["AdminCredentials:Password"];

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return;
    }

    try
    {
        var context = scope.ServiceProvider.GetRequiredService<Cabeleleira_LeilaDbContext>();
        var exists = context.Users.Any(user => user.Email == email);

        if (exists)
        {
            return;
        }

        var name = configuration["AdminCredentials:Name"] ?? "Administrativo Leila";
        var number = configuration["AdminCredentials:Number"] ?? string.Empty;
        var passwordHashService = scope.ServiceProvider.GetRequiredService<IPasswordHashService>();
        var admin = new User(name, number, email, passwordHashService.Hash(password), UserRole.Admin);

        context.Users.Add(admin);
        context.SaveChanges();
    }
    catch (Exception exception)
    {
        logger.LogWarning(exception, "Nao foi possivel criar o usuario administrador inicial.");
    }
}
