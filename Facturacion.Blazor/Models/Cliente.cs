using System.ComponentModel.DataAnnotations;

namespace Facturacion.Blazor.Models;

public class Cliente
{
    [Required(ErrorMessage = "La cédula/RUC es obligatoria")]
    [StringLength(13, MinimumLength = 10, ErrorMessage = "Debe tener entre 10 y 13 dígitos")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Correo no válido")]
    [StringLength(100)]
    public string? Correo { get; set; }

    [Phone(ErrorMessage = "Teléfono no válido")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(200)]
    public string? Direccion { get; set; }
}
