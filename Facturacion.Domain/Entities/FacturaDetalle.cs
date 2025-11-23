// FacturaDetalle.cs
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Domain.Entities;

public class FacturaDetalle : BaseEntity
{
    public int Id { get; set; }

    public int FacturaId { get; set; }
    public virtual Factura Factura { get; set; } = null!;

    public int ProductoId { get; set; }
    public virtual Producto Producto { get; set; } = null!;

    public int? ProductoLoteId { get; set; }
    public virtual ProductoLote? Lote { get; set; }

    public int Cantidad { get; set; } = 1;

    public decimal PrecioUnitario { get; set; }
    public decimal SubtotalLinea { get; set; }
    public decimal PorcentajeIva { get; set; } = 15;
    public decimal ValorIva { get; set; }
    public decimal TotalLinea { get; set; }
}