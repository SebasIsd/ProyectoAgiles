using Facturacion.Application.Interfaces;
using Facturacion.Infrastructure.Persistence;
using Facturacion.Infrastructure.Services;
using Facturacion.Domain.Entities;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDB")));

builder.Services.AddScoped<IAuthService, AuthService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

const string CorsPolicy = "_blazor";
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p =>
    p.WithOrigins("http://localhost:5062")
     .AllowAnyHeader()
     .AllowAnyMethod()
));

var app = builder.Build();

try
{
    string targetDb = "FacturacionSRI";
    string sqlExpressConn = "Server=(local)\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

    using (var conn = new SqlConnection(sqlExpressConn))
    {
        conn.Open();

        var checkCmd = new SqlCommand($"SELECT DB_ID('{targetDb}')", conn);
        var exists = checkCmd.ExecuteScalar();

        if (exists == DBNull.Value || exists == null)
        {
            string scriptPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "ProyectoAgiles",
                "db",
                "FacturacionSRI_Sprint1.sql"
            );

            if (File.Exists(scriptPath))
            {
                Console.WriteLine($"🟢 Ejecutando script SQL desde: {scriptPath}");
                string fullScript = File.ReadAllText(scriptPath);

                var batches = Regex.Split(
                    fullScript,
                    @"^\s*GO\s*?$",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase
                );

                int i = 0;
                foreach (var batch in batches)
                {
                    var sql = batch.Trim();
                    if (string.IsNullOrWhiteSpace(sql)) continue;
                    i++;

                    try
                    {
                        using var cmd = new SqlCommand(sql, conn);
                        cmd.CommandTimeout = 600;
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"✅ Lote {i} ejecutado correctamente.");
                    }
                    catch (Exception exBatch)
                    {
                        Console.WriteLine($"❌ Error en lote {i}: {exBatch.Message}");
                    }
                }

                Console.WriteLine("✅ Script SQL ejecutado completamente y base creada con datos.");
            }
            else
            {
                Console.WriteLine($"⚠️ No se encontró el script SQL en: {scriptPath}");
            }
        }
        else
        {
            Console.WriteLine($"ℹ️ La base de datos '{targetDb}' ya existe. No se ejecutó el script.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Error al ejecutar script SQL: {ex.Message}");
}


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    var admin = context.Usuarios.FirstOrDefault(u => u.Username == "admin");
    if (admin == null)
    {
        admin = new Usuario
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin",
            Nombre = "Administrador del Sistema",
            Email = "admin@factura.ec"
        };
        context.Usuarios.Add(admin);
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/ping", () => Results.Ok("pong"));

app.Run();
