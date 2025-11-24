// Facturacion.Application/DTOs/LoteDto.cs
using System;

namespace Facturacion.Application.DTOs
{
    public class LoteDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Lote { get; set; } = string.Empty;
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public bool Activo { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoCodigo { get; set; } = string.Empty;
        public string ProductoCategoria { get; set; } = string.Empty;
    }

    public class CreateLoteDto
    {
        public int ProductoId { get; set; }
        public string Lote { get; set; } = string.Empty;
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public DateTime FechaIngreso { get; set; } = DateTime.Now.Date;
        public DateTime? FechaVencimiento { get; set; }
        public int CreatedBy { get; set; } = 1; // Por defecto admin
    }

    public class UpdateLoteDto
    {
        public int Id { get; set; }
        public string Lote { get; set; } = string.Empty;
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public bool Activo { get; set; }
    }
}