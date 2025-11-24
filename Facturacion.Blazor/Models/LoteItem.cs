// Facturacion.Blazor/Models/LoteItem.cs
using System;

namespace Facturacion.Blazor.Models
{
    public class LoteItem
    {
        public int LoteId { get; set; }
        public string ProductoCodigo { get; set; } = string.Empty;
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoCategoria { get; set; } = string.Empty;
        public DateTime FechaCompra { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PVP { get; set; }
        public int CantidadInicial { get; set; }
        public int CantidadDisponible { get; set; }
        public string Estado { get; set; } = "Activo";
        
        // Propiedad adicional para el mapeo desde el backend
        public int ProductoId { get; set; }
    }
}