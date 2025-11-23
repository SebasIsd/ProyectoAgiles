using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Application.DTOs
{
    public class CrearFacturaDto
    {
        public int ClienteId { get; set; }
        public List<CrearFacturaDetalleDto> Detalles { get; set; } = new();
    }

    public class CrearFacturaDetalleDto
    {
        public int ProductoId { get; set; }
        public int? ProductoLoteId { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}
