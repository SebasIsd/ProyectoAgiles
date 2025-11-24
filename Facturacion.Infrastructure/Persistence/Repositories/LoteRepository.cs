// Facturacion.Infrastructure/Repositories/LoteRepository.cs
using Microsoft.EntityFrameworkCore;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;
using Facturacion.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facturacion.Infrastructure.Repositories
{
    public class LoteRepository : ILoteRepository
    {
        private readonly AppDbContext _context;

        public LoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoLote>> GetAllAsync()
        {
            return await _context.ProductoLotes
                .Include(p => p.Producto)
                .ThenInclude(p => p.Categoria)
                .Where(l => l.Activo)
                .OrderByDescending(l => l.Id)
                .ToListAsync();
        }

        public async Task<ProductoLote?> GetByIdAsync(int id)
        {
            return await _context.ProductoLotes
                .Include(p => p.Producto)
                .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(l => l.Id == id && l.Activo);
        }

        public async Task<ProductoLote> AddAsync(ProductoLote lote)
        {
            _context.ProductoLotes.Add(lote);
            await _context.SaveChangesAsync();
            return lote;
        }

        public async Task<ProductoLote> UpdateAsync(ProductoLote lote)
        {
            _context.ProductoLotes.Update(lote);
            await _context.SaveChangesAsync();
            return lote;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lote = await _context.ProductoLotes.FindAsync(id);
            if (lote == null) return false;

            lote.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductoLote>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.Trim().ToLower();

            return await _context.ProductoLotes
                .Include(p => p.Producto)
                .ThenInclude(p => p.Categoria)
                .Where(l => l.Activo &&
                           (l.Producto.Nombre.ToLower().Contains(term) ||
                            l.Producto.Codigo != null && l.Producto.Codigo.ToLower().Contains(term) ||
                            l.Lote.ToLower().Contains(term)))
                .ToListAsync();
        }

        public async Task<bool> LoteExistsAsync(int productoId, string lote, int? excludeId = null)
        {
            var query = _context.ProductoLotes
                .Where(l => l.ProductoId == productoId && 
                           l.Lote.ToLower() == lote.ToLower() && 
                           l.Activo);

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}