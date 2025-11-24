// Facturacion.Application/DTOs/ProductoComboDto.cs
namespace Facturacion.Application.DTOs
{
    public class ProductoComboDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Categoria { get; set; }
    }
}