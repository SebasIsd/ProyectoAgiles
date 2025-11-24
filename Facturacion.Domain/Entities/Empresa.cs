using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Domain.Entities;

public class Empresa
{
    public int Id { get; set; }

    [Required]
    [StringLength(13)]
    public string Ruc { get; set; } = null!;

    [Required]
    [StringLength(300)]
    public string RazonSocial { get; set; } = null!;

    [StringLength(300)]
    public string? NombreComercial { get; set; }

    [StringLength(300)]
    public string? DireccionMatriz { get; set; }

    [StringLength(10)]
    public string? ContribuyenteEspecial { get; set; }

    public bool ObligadoContabilidad { get; set; } = true;

    [StringLength(500)]
    public string? LogoPath { get; set; }

    public bool Activo { get; set; } = true;

    // Auditoría
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}