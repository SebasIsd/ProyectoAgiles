using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;
using Facturacion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly AppDbContext _context;

    public ProductoRepository(AppDbContext context) => _context = context;

    // 🔥 AHORA CON Marca y Categoria
    public async Task<List<Producto>> GetAllActivosConLotesAsync() =>
        await _context.Productos
            .Where(p => p.Activo)
            .Include(p => p.Marca)                     // ← RELACIÓN MARCA
            .Include(p => p.Categoria)                 // ← RELACIÓN CATEGORÍA
            .Include(p => p.Lotes.Where(l => l.Activo))// ← LOTES ACTIVOS
            .OrderBy(p => p.Nombre)
            .ToListAsync();

    // 🔥 AHORA CON Marca y Categoria
    public async Task<Producto?> GetByIdConLotesAsync(int id) =>
        await _context.Productos
            .Include(p => p.Marca)                     // ← RELACIÓN MARCA
            .Include(p => p.Categoria)                 // ← RELACIÓN CATEGORÍA
            .Include(p => p.Lotes.Where(l => l.Activo))// ← LOTES ACTIVOS
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

    public async Task<Producto?> GetByCodigoAsync(string codigo) =>
        await _context.Productos
            .Include(p => p.Marca)                     // ✔ opcional pero recomendado
            .Include(p => p.Categoria)                 // ✔ opcional pero recomendado
            .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Activo);

    public async Task AddAsync(Producto producto)
    {
        await _context.Productos.AddAsync(producto);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        var p = await _context.Productos.FindAsync(id);
        if (p != null)
        {
            p.Activo = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddLoteAsync(ProductoLote lote)
    {
        await _context.ProductoLotes.AddAsync(lote);
        await _context.SaveChangesAsync();
    }

    public async Task<ProductoLote?> GetLoteAsync(int productoId, string lote) =>
        await _context.ProductoLotes
            .FirstOrDefaultAsync(pl => pl.ProductoId == productoId && pl.Lote == lote && pl.Activo);

    public async Task<List<CategoriaProducto>> GetCategoriasAsync()
    {
        return await _context.CategoriaProductos
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }
}
