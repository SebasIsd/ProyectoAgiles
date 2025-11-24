using Facturacion.Application.DTOs;

namespace Facturacion.Application.Interfaces
{
    public interface ICobroService
    {
        Task<IEnumerable<CobroDto>> GetAll();
        Task<CobroDto?> GetById(int id);
        Task<CobroDto> Create(CrearCobroDto dto);
        Task<bool> Delete(int id);
    }
}
