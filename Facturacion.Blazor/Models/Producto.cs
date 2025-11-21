using System.ComponentModel.DataAnnotations;

namespace Facturacion.Blazor.Models;

public class Producto
{
    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; } = 0;

    [Display(Name = "Fecha de expiración")]
    public DateTime? FechaExpiracion { get; set; }
}
