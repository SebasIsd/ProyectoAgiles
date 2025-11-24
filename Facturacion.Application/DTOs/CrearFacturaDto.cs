using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Ingrese un numero de cantidad es obligatoria")]
        public int Cantidad { get; set; } = 1;
    }
}
