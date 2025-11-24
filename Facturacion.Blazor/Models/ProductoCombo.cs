// Facturacion.Blazor/Models/ProductoCombo.cs
namespace Facturacion.Blazor.Models
{
    public class ProductoCombo
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Categoria { get; set; }
    }
}