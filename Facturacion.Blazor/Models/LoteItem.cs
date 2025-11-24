// Facturacion.Blazor/Models/LoteItem.cs
public class LoteItem
{
    public int Id { get; set; }  // Cambiar de LoteId a Id
    public int ProductoId { get; set; }
    public string Lote { get; set; } = string.Empty;
    public decimal PrecioCompra { get; set; }  // Cambiar de PrecioCosto
    public decimal PrecioVenta { get; set; }   // Cambiar de PVP
    public int Stock { get; set; }             // Cambiar de CantidadInicial/Disponible
    public DateTime FechaIngreso { get; set; } // Cambiar de FechaCompra
    public DateTime? FechaVencimiento { get; set; } // Cambiar de FechaExpiracion
    public bool Activo { get; set; }           // Cambiar de Estado

    // Propiedades desde JOIN (deben venir del backend)
    public string? ProductoNombre { get; set; }
    public string? ProductoCodigo { get; set; }
    public string? ProductoCategoria { get; set; }

    // Propiedades calculadas para la UI
    public string Estado => Activo ? "Activo" : "Inactivo";
    public int CantidadInicial => Stock;
    public int CantidadDisponible => Stock;
    public decimal PrecioCosto => PrecioCompra;
    public decimal PVP => PrecioVenta;
    public DateTime FechaCompra => FechaIngreso;
    public DateTime? FechaExpiracion => FechaVencimiento;
}