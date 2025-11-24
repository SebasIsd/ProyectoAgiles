// Facturacion.Domain/Interfaces/ILoteRepository.cs
using Facturacion.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facturacion.Domain.Interfaces
{
    public interface ILoteRepository
    {
        Task<IEnumerable<ProductoLote>> GetAllAsync();
        Task<ProductoLote?> GetByIdAsync(int id);
        Task<ProductoLote> AddAsync(ProductoLote lote);
        Task<ProductoLote> UpdateAsync(ProductoLote lote);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProductoLote>> SearchAsync(string searchTerm);
        Task<bool> LoteExistsAsync(int productoId, string lote, int? excludeId = null);
    }
}