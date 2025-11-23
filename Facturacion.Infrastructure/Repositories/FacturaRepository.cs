using Facturacion.Domain.Entities; 
using Facturacion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Facturacion.Infrastructure.Repositories;
public class FacturaRepository : IFacturaRepository
{
    private readonly AppDbContext _context;

    public FacturaRepository(AppDbContext context) => _context = context;

    public async Task<List<Factura>> GetAllAsync()
    {
        return await _context.Facturas
            .Include(f => f.Cliente)
            .Include(f => f.Detalles)
            .ThenInclude(d => d.ProductoLote)
            .ToListAsync();
    }

    public async Task<Factura?> GetByIdAsync(int id)
    {
        return await _context.Facturas
            .Include(f => f.Cliente)
            .Include(f => f.Detalles)
            .ThenInclude(d => d.ProductoLote)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Factura> CreateAsync(Factura factura)
    {
        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync();
        return factura;
    }

    public async Task UpdateAsync(Factura factura)
    {
        _context.Facturas.Update(factura);
        await _context.SaveChangesAsync();
    }
}
