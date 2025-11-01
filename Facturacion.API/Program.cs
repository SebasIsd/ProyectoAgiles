using Facturacion.Application.Interfaces;
using Facturacion.Infrastructure.Persistence;
using Facturacion.Infrastructure.Services;
using Facturacion.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDB")));

// Auth
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
var key = Encoding.ASCII.GetBytes(jwtKey);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Startup seed + fix de contraseña:
// - Si no existe admin: lo crea con contraseña hasheada "Admin123!"
// - Si existe admin pero su PasswordHash no parece un hash BCrypt, lo rehasea y lo actualiza.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Aplica migraciones pendientes (si usas migrations)
    try
    {
        context.Database.Migrate();
    }
    catch
    {
        // no bloquear en caso de que no quieras aplicar migraciones automáticamente
    }

    var admin = context.Usuarios.FirstOrDefault(u => u.Username == "admin");
    if (admin == null)
    {
        admin = new Usuario
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin",
            Nombre = "Administrador",
            Email = "admin@factura.ec"
        };
        context.Usuarios.Add(admin);
        context.SaveChanges();
    }
    else
    {
        // Si la contraseña guardada NO parece un hash BCrypt (p. ej. no empieza por $2), la rehasheamos.
        if (string.IsNullOrWhiteSpace(admin.PasswordHash) || !admin.PasswordHash.StartsWith("$2"))
        {
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            context.Usuarios.Update(admin);
            context.SaveChanges();
        }
    }
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();