using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Facturacion.Application/DTOs/ProductoDtos.cs

namespace Facturacion.Application.DTOs;

public class CrearProductoDto
{
    public string Nombre { get; set; } = null!;
    public string? Codigo { get; set; }
    public string? CodigoBarra { get; set; }
    public string? Descripcion { get; set; }
    public string? Modelo { get; set; }
    public int? MarcaId { get; set; }        // ← NUEVO
    public int? CategoriaId { get; set; }    // ← NUEVO
}
public class ActualizarProductoDto
{
    public string Nombre { get; set; } = null!;
    public string? Codigo { get; set; }
    public string? CodigoBarra { get; set; }
    public string? Descripcion { get; set; }
    public string? Modelo { get; set; }
    public int? MarcaId { get; set; }        // ← NUEVO
    public int? CategoriaId { get; set; }    // ← NUEVO
}
public class CrearLoteDto
{
    public int ProductoId { get; set; }
    public string Lote { get; set; } = string.Empty;
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int Cantidad { get; set; } = 0;
    public DateTime? FechaVencimiento { get; set; }
}

public class ProductoListaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Codigo { get; set; }

    // CAMBIO AQUÍ: una sola propiedad para mostrar
    public string MarcaModelo => (Marca + " " + Modelo).Trim() == ""
        ? "Sin marca / modelo"
        : (Marca + " " + Modelo).Trim();

    public string? Marca { get; set; }   // viene de p.Marca?.Nombre
    public string? Modelo { get; set; }  // viene de p.Modelo

    public int StockTotal { get; set; }
    public decimal PrecioVentaActual { get; set; }
    public bool Activo { get; set; }
    public String DescripcionCorta
    {
        get
        {
            if (string.IsNullOrEmpty(Descripcion))
                return "Sin descripción";
            if (Descripcion.Length <= 50)
                return Descripcion;
            return Descripcion.Substring(0, 50) + "...";
        }
    }

    public string? Descripcion { get; private set; }
}

public class ProductoDetalleDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Codigo { get; set; }
    public string? CodigoBarra { get; set; }
    public string? Descripcion { get; set; }
    public string? Modelo { get; set; }

    // ← ESTOS DOS SON LOS QUE TE FALTABAN
    public MarcaDto? Marca { get; set; }
    public CategoriaDto? Categoria { get; set; }

    public List<ProductoLoteDto> Lotes { get; set; } = new();
}

public class ProductoLoteDto
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

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

