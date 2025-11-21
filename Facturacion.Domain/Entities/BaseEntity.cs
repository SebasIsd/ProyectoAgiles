using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Domain.Entities;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }

    public int CreatedBy { get; set; } = 1; // 1 = sistema/admin
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool Activo { get; set; } = true;
}