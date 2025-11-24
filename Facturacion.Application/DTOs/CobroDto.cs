namespace Facturacion.Application.DTOs
{
    public class CobroDto
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }

        // AQUI falta esta propiedad
        public string NumeroFactura { get; set; } = string.Empty;

        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }

    public class CrearCobroDto
    {
        public int FacturaId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
    }
}
