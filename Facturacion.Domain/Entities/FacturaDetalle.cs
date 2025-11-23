public class FacturaDetalle : BaseEntity
{
    public int FacturaId { get; set; }
    public Factura Factura { get; set; } = null!;

    public int ProductoLoteId { get; set; }
    public ProductoLote ProductoLote { get; set; } = null!;

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
}
