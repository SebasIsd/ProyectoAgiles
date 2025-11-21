using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Codigo { get; set; }
    public string? CodigoBarra { get; set; }
    public string? Descripcion { get; set; }

    // NUEVAS PROPIEDADES DE NAVEGACIÓN
    public int? MarcaId { get; set; }
    public Marca? Marca { get; set; }           // ← objeto completo

    public int? CategoriaId { get; set; }
    public CategoriaProducto? Categoria { get; set; }

    // Modelo ya no es string, ahora también es una entidad (o lo dejamos como string si prefieres)
    // Opción A: lo dejamos como string (más simple y común)
    public string? Modelo { get; set; }         // ← sigue siendo string

    // Opción B: si también quieres que Modelo sea una tabla, avísame y lo hacemos.

    public bool Activo { get; set; } = true;
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relación con lotes
    public List<ProductoLote> Lotes { get; set; } = new();
}