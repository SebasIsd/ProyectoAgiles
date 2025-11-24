using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Archivo: Facturacion.Application/DTOs/ClienteDtos.cs
// Facturacion.Application/DTOs/ClienteDtos.cs

namespace Facturacion.Application.DTOs;

// Usamos CLASS en lugar de record para permitir setters normales
public class CrearClienteDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El tipo de identificación es obligatorio")]
    public string TipoIdentificacion { get; set; } = "CEDULA"; // valor por defecto
    [Required(ErrorMessage = "La identificación es obligatoria")]
    [RegularExpression(@"^\d+$", ErrorMessage = "La identificación debe contener solo números")]
    [StringLength(13, MinimumLength = 5, ErrorMessage = "La identificación debe tener entre 5 y 13 dígitos")]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Su direccion es obligatoria")]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "El telefono es obligatorio")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Su correo es obligatorio")]
    public string? Email { get; set; }
}

public class ActualizarClienteDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Su direccion es obligatoria")]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "El telefono es obligatorio")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Su correo es obligatorio")]
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