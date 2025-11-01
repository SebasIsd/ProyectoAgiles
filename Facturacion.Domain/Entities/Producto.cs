using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Domain.Entities
{
    public class Producto : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string CodigoBarra { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }

        public int Stock { get; set; } = 0;

        public DateTime? FechaVencimiento { get; set; }
    }
}