using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Application.DTOs
{
    public class FacturaDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public int ClienteId { get; set; }  // AGREGADO para editar
        public string ClienteNombre { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public List<FacturaDetalleDto> Detalles { get; set; } = new();
    }

    public class FacturaDetalleDto
    {
        public int ProductoId { get; set; }  // AGREGADO para editar
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubtotalLinea { get; set; }  // AGREGADO para detalle
        public decimal ValorIva { get; set; }  // AGREGADO para detalle
        public decimal TotalLinea { get; set; }
    }
}
