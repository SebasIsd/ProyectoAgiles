public interface IFacturaRepository
{
    Task<Factura?> GetByIdAsync(int id);
    Task<List<Factura>> GetAllAsync();
    Task<Factura> CreateAsync(Factura factura);
    Task UpdateAsync(Factura factura);
}
