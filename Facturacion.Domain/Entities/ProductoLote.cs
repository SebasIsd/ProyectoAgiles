using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Domain.Entities;

public class ProductoLote : BaseEntity
{
    public int ProductoId { get; set; }
    public virtual Producto Producto { get; set; } = null!;

    [Required, StringLength(50)]
    public string Lote { get; set; } = string.Empty;

    [Range(0.01, 999999.99)]
    public decimal PrecioCompra { get; set; }

    [Range(0.01, 999999.99)]
    public decimal PrecioVenta { get; set; }

    public int Stock { get; set; } = 0;

    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow.Date;

    public DateTime? FechaVencimiento { get; set; }

    public bool Activo { get; set; } = true;

    public int CreatedBy { get; set; }
}