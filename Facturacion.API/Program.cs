// Program.cs → Versión 100% corregida y funcional (2025)
using Facturacion.Application.Interfaces;
using Facturacion.Application.Services;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;
using Facturacion.Infrastructure.Persistence;
using Facturacion.Infrastructure.Persistence.Repositories;
using Facturacion.Infrastructure.Repositories;
using Facturacion.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esta instrucción le dice a la API: "Si ves un objeto repetido (bucle), ignóralo"
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDB")));

// Repositorios y servicios
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ILoteRepository, LoteRepository>();
builder.Services.AddScoped<ILoteService, LoteService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IFacturaService, FacturaService>();

// JWT (no toco tu configuración, solo la dejo igual)
var jwtKey = builder.Configuration["Jwt:Key"]!;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(o => o.AddPolicy("_blazor", p =>
    p.WithOrigins("http://localhost:5062", "https://localhost:7200", "http://localhost:5183")
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("_blazor");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ===================================================
// SEMBRADO DE DATOS DE PRUEBA (CORREGIDO AL 100%)
// ===================================================
/*using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    // 1. Usuario Admin
    if (!context.Usuarios.Any())
    {
        context.Usuarios.Add(new Usuario
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", 10),
            Role = "Admin",
            Nombre = "Administrador del Sistema",
            Email = "admin@factura.ec"
        });
        await context.SaveChangesAsync();
        Console.WriteLine("Usuario admin creado");
    }

    // 3. Marcas
    if (!context.Marca.Any())
    {
        context.Marca.AddRange(
            new Marca { Nombre = "Samsung" },
            new Marca { Nombre = "Sony" },
            new Marca { Nombre = "Dell" },
            new Marca { Nombre = "Indurama" },
            new Marca { Nombre = "Umco" },  
            new Marca { Nombre = "Mabe" }
        );
        await context.SaveChangesAsync();
    }

    // 4. Categorías de producto
    if (!context.Categorias.Any())
    {
        context.Categorias.AddRange(
            new CategoriaProducto { Nombre = "Electrodomésticos" },
            new CategoriaProducto { Nombre = "Alimentos" },
            new CategoriaProducto { Nombre = "Tecnología" },
            new CategoriaProducto { Nombre = "Línea Blanca" }
        );
        await context.SaveChangesAsync();
    }

    // 5. Productos + Lotes de ejemplo
    if (!context.Productos.Any())
    {
        var marcaSamsung = await context.Marca.FirstAsync(m => m.Nombre == "Samsung");
        var catAlimentos = await context.Categorias.FirstAsync(c => c.Nombre == "Alimentos");
        var catTecnologia = await context.Categorias.FirstAsync(c => c.Nombre == "Tecnología");

        var productos = new[]
        {
            new Producto
            {
                Nombre = "Arroz Diana 5kg",
                Codigo = "ARZ001",
                CodigoBarra = "7751234567890",
                CategoriaId = catAlimentos.Id,
                Modelo = "Pack familiar",               // Modelo sigue siendo string
                CreatedBy = 1,
                Activo = true
            },
            new Producto
            {
                Nombre = "Aceite Premier 1L",
                Codigo = "ACE001",
                CodigoBarra = "7701234567890",
                CategoriaId = catAlimentos.Id,
                CreatedBy = 1,
                Activo = true
            },
            new Producto
            {
                Nombre = "TV Samsung 55\" 4K",
                Codigo = "TV001",
                CodigoBarra = "8801234567890",
                MarcaId = marcaSamsung.Id,
                CategoriaId = catTecnologia.Id,
                Modelo = "UN55AU8000",
                CreatedBy = 1,
                Activo = true
            }
        };

        context.Productos.AddRange(productos);
        await context.SaveChangesAsync();

        // Lotes de ejemplo
        var arroz = await context.Productos.FirstAsync(p => p.Codigo == "ARZ001");
        context.ProductoLotes.AddRange(
            new ProductoLote
            {
                ProductoId = arroz.Id,
                Lote = "L2025-001",
                PrecioCompra = 4.50m,
                PrecioVenta = 6.80m,
                Stock = 120,
                FechaIngreso = DateTime.Today.AddDays(-30),
                FechaVencimiento = DateTime.Today.AddYears(1),
                CreatedBy = 1,
                Activo = true
            },
            new ProductoLote
            {
                ProductoId = arroz.Id,
                Lote = "L2025-002",
                PrecioCompra = 4.70m,
                PrecioVenta = 7.00m,
                Stock = 80,
                FechaIngreso = DateTime.Today.AddDays(-10),
                CreatedBy = 1,
                Activo = true
            }
        );
        await context.SaveChangesAsync();
    }

    // 6. Clientes de ejemplo
    // 6. Clientes de ejemplo (sin tocar ninguna tabla de tipos)
    if (!context.Clientes.Any())
    {
        context.Clientes.AddRange(
            new Cliente
            {
                Nombre = "Juan Pérez",
                TipoIdentificacion = TipoIdentificacion.CEDULA,
                Identificacion = "0901234567",
                Direccion = "Av. Amazonas",
                Telefono = "0991234567",
                Email = "juan@email.com",
                CreatedBy = 1
            },
            new Cliente
            {
                Nombre = "Distribuidora Norte",
                TipoIdentificacion = TipoIdentificacion.RUC,
                Identificacion = "1791234567001",
                Direccion = "Quito",
                Telefono = "022345678",
                Email = "info@norte.com",
                CreatedBy = 1
            },
            new Cliente
            {
                Nombre = "María López",
                TipoIdentificacion = TipoIdentificacion.CEDULA,
                Identificacion = "0956789012",
                CreatedBy = 1
            },
            new Cliente
            {
                Nombre = "Supermercados Tía",
                TipoIdentificacion = TipoIdentificacion.RUC,
                Identificacion = "1799876543001",
                Direccion = "Guayaquil",
                CreatedBy = 1
            }
        );
        await context.SaveChangesAsync();
        Console.WriteLine("Clientes de ejemplo creados");
    }

    if (!context.Secuenciales.Any())
    {
        context.Secuenciales.Add(new Secuencial
        {
            Establecimiento = "001",
            PuntoEmision = "001",
            SecuencialActual = 0,
            TipoComprobante = "01" // Tipo de comprobante para facturas
        });
        await context.SaveChangesAsync();
        Console.WriteLine("Secuencial inicial creado.");
    }
}*/

app.MapGet("/ping", () => "API Facturación SRI - Todo OK!");

app.Run();