// Facturacion.Domain/Entities/ProductoLote.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturacion.Domain.Entities
{
    public class ProductoLote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Lote { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioCompra { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioVenta { get; set; }

        [Required]
        public int Stock { get; set; } = 0;

        [Required]
        public DateTime FechaIngreso { get; set; } = DateTime.Now.Date;

        public DateTime? FechaVencimiento { get; set; }

        [Required]
        public bool Activo { get; set; } = true;

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegación - CORREGIDO para que coincida con tu Producto
        public virtual Producto Producto { get; set; } = null!;
    }
}