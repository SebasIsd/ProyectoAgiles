using Facturacion.Application.DTOs;
namespace Facturacion.Application.Interfaces
{
public interface IFacturaService
{
    Task<FacturaDto> CreateAsync(FacturaDto dto);
    Task<FacturaDto?> GetByIdAsync(int id);
    Task UpdateAsync(FacturaDto dto);
}
}