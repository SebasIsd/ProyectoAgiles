using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Facturacion.Domain.Entities;

namespace Facturacion.Domain.Interfaces;

public interface IProductoRepository
{
    Task<List<Producto>> GetAllActivosConLotesAsync();
    Task<Producto?> GetByIdConLotesAsync(int id);
    Task<Producto?> GetByCodigoAsync(string codigo);
    Task AddAsync(Producto producto);
    Task UpdateAsync(Producto producto);
    Task DesactivarAsync(int id);
    Task AddLoteAsync(ProductoLote lote);
    Task<ProductoLote?> GetLoteAsync(int productoId, string lote);
    Task<List<CategoriaProducto>> GetCategoriasAsync();
}