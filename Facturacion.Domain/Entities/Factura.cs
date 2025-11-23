// Factura.cs
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Domain.Entities;

public class Factura : BaseEntity
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Numero { get; set; } = string.Empty; // 001-001-000000001

    public DateTime FechaEmision { get; set; } = DateTime.Today;

    public int ClienteId { get; set; }
    public virtual Cliente Cliente { get; set; } = null!;

    public decimal Subtotal { get; set; }
    public decimal TotalDescuento { get; set; } = 0;
    public decimal Iva { get; set; }
    public decimal Total { get; set; }

    [StringLength(20)]
    public string Estado { get; set; } = "Borrador"; // Borrador, Emitida, Autorizada, etc.

    public int CreatedBy { get; set; }
    //public virtual Usuario CreatedByUser { get; set; } = null!;

    // Navegación
    public virtual ICollection<FacturaDetalle> Detalles { get; set; } = new List<FacturaDetalle>();
}