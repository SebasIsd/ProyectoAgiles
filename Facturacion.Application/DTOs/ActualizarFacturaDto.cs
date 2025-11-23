using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Application.Dtos;

public class ActualizarFacturaDto
{
    public int ClienteId { get; set; }
    public List<ActualizarFacturaDetalleDto> Detalles { get; set; } = new();
}

public class ActualizarFacturaDetalleDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; } = 1;
}