using Facturacion.Domain.Entities;

namespace Facturacion.Domain.Entities
{
    public class Cobro
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }

        public Factura? Factura { get; set; }
    }
}
