using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Archivo: Facturacion.Application/DTOs/ClienteDtos.cs
// Facturacion.Application/DTOs/ClienteDtos.cs

namespace Facturacion.Application.DTOs;

// Usamos CLASS en lugar de record para permitir setters normales
public class CrearClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string TipoIdentificacion { get; set; } = "CEDULA"; // valor por defecto
    public string Identificacion { get; set; } = string.Empty;

    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}

public class ActualizarClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}

// ClienteDto sí puede seguir siendo record (solo lectura, ideal para mostrar)
public record ClienteDto(
    int Id,
    string Nombre,
    string TipoIdentificacion,
    string Identificacion,
    string? Email,
    string? Telefono,
    string? Direccion,
    bool Activo);