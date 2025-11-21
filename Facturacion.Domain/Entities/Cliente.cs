using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


using System.ComponentModel.DataAnnotations.Schema;

namespace Facturacion.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;

    public int TipoIdentificacionId { get; set; }        // ← FK
    public TipoIdentificacion TipoIdentificacion { get; set; } = TipoIdentificacion.CEDULA; // navegación

    public string Identificacion { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}