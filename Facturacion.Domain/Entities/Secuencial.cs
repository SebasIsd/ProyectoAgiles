using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Facturacion.Domain.Entities/Secuencial.cs

using System.ComponentModel.DataAnnotations; // Agrega esto para el StringLength

namespace Facturacion.Domain.Entities;

public class Secuencial
{
    public int Id { get; set; }

    // Agrega la anotación para que EF Core sepa el tamaño
    [Required, StringLength(2)]
    public string TipoComprobante { get; set; } = "01"; // <-- ¡NUEVA PROPIEDAD!

    public string Establecimiento { get; set; } = "001";
    public string PuntoEmision { get; set; } = "001";
    public int SecuencialActual { get; set; } = 0;
}