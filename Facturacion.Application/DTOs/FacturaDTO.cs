namespace Facturacion.Application.DTOs
{
    public class FacturaDto
    {
        public int Id { get; set; }

        public string NumeroFactura { get; set; } = string.Empty;

        public DateTime FechaEmision { get; set; }

        public int ClienteId { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public List<FacturaDetalleDTO>? Detalles { get; set; }
    }
}
