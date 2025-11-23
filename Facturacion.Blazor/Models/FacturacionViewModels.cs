using System;
using System.Collections.Generic;
using Facturacion.Blazor.Models;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Blazor.Models
{
    // Factura para la UI / API from Blazor
    public class FacturaVm
    {
        public int? Id { get; set; }
        public string? Numero { get; set; }

        [Required]
        public int? ClienteId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Today;

        // IVA
        public int IvaPorcentajeSeleccionado { get; set; } = 15;
        public bool UsarIvaPersonalizado { get; set; }
        public decimal? IvaPorcentajePersonalizado { get; set; }
        public string? JustificacionIva { get; set; }
        public decimal IvaAplicadoPorcentaje { get; set; }
        public decimal IvaMonto { get; set; }

        // Totales
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }

        // Líneas
        public List<FacturaDetalleVm> Detalles { get; set; } = new();

        // Opcional: estado, creador, etc.
        public string? Estado { get; set; }
    }

    public class FacturaDetalleVm
    {
        public int? Id { get; set; }

        [Required]
        public int? ProductoLoteId { get; set; }

        public int Cantidad { get; set; } = 1;

        // Precio y subtotal calculados en cliente
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }

        // Campos extras si necesitas enviar IVA por línea
        public decimal IvaLinea { get; set; }
        public decimal TotalLinea { get; set; }
    }

    // Modelo mínimo de Cliente para dropdown
    public class ClienteVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        // agrega lo que necesites (Identificacion, Email...)
    }

    // Lote que trae la API para seleccionar precio/stock
    public class LoteVm
    {
        public int Id { get; set; }
        public string Lote { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public ProductoVm Producto { get; set; } = new ProductoVm();
    }

    public class ProductoVm
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
