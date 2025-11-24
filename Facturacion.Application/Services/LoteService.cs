// Facturacion.Application/Services/LoteService.cs
using Facturacion.Application.DTOs;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facturacion.Application.Services
{
    public interface ILoteService
    {
        Task<IEnumerable<LoteDto>> GetAllAsync();
        Task<LoteDto?> GetByIdAsync(int id);
        Task<LoteDto> CreateAsync(CreateLoteDto createDto);
        Task<LoteDto> UpdateAsync(UpdateLoteDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<LoteDto>> SearchAsync(string searchTerm);
    }

    public class LoteService : ILoteService
    {
        private readonly ILoteRepository _loteRepository;

        public LoteService(ILoteRepository loteRepository)
        {
            _loteRepository = loteRepository;
        }

        public async Task<IEnumerable<LoteDto>> GetAllAsync()
        {
            var lotes = await _loteRepository.GetAllAsync();
            return lotes.Select(MapToDto);
        }

        public async Task<LoteDto?> GetByIdAsync(int id)
        {
            var lote = await _loteRepository.GetByIdAsync(id);
            return lote != null ? MapToDto(lote) : null;
        }

        public async Task<LoteDto> CreateAsync(CreateLoteDto createDto)
        {
            // Validar que no exista un lote con el mismo código para el mismo producto
            var exists = await _loteRepository.LoteExistsAsync(createDto.ProductoId, createDto.Lote);
            if (exists)
            {
                throw new System.Exception("Ya existe un lote con el mismo código para este producto.");
            }

            var lote = new ProductoLote
            {
                ProductoId = createDto.ProductoId,
                Lote = createDto.Lote,
                PrecioCompra = createDto.PrecioCompra,
                PrecioVenta = createDto.PrecioVenta,
                Stock = createDto.Stock,
                FechaIngreso = createDto.FechaIngreso,
                FechaVencimiento = createDto.FechaVencimiento,
                CreatedBy = createDto.CreatedBy,
                Activo = true,
                CreatedAt = DateTime.Now
            };

            var created = await _loteRepository.AddAsync(lote);
            return MapToDto(created);
        }

        public async Task<LoteDto> UpdateAsync(UpdateLoteDto updateDto)
        {
            var lote = await _loteRepository.GetByIdAsync(updateDto.Id);
            if (lote == null)
            {
                throw new System.Exception("Lote no encontrado.");
            }

            // Validar que no exista otro lote con el mismo código para el mismo producto
            var exists = await _loteRepository.LoteExistsAsync(lote.ProductoId, updateDto.Lote, updateDto.Id);
            if (exists)
            {
                throw new System.Exception("Ya existe otro lote con el mismo código para este producto.");
            }

            lote.Lote = updateDto.Lote;
            lote.PrecioCompra = updateDto.PrecioCompra;
            lote.PrecioVenta = updateDto.PrecioVenta;
            lote.Stock = updateDto.Stock;
            lote.FechaIngreso = updateDto.FechaIngreso;
            lote.FechaVencimiento = updateDto.FechaVencimiento;
            lote.Activo = updateDto.Activo;

            var updated = await _loteRepository.UpdateAsync(lote);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _loteRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<LoteDto>> SearchAsync(string searchTerm)
        {
            var lotes = await _loteRepository.SearchAsync(searchTerm);
            return lotes.Select(MapToDto);
        }

        private static LoteDto MapToDto(ProductoLote lote)
        {
            return new LoteDto
            {
                Id = lote.Id,
                ProductoId = lote.ProductoId,
                Lote = lote.Lote,
                PrecioCompra = lote.PrecioCompra,
                PrecioVenta = lote.PrecioVenta,
                Stock = lote.Stock,
                FechaIngreso = lote.FechaIngreso,
                FechaVencimiento = lote.FechaVencimiento,
                Activo = lote.Activo,
                ProductoNombre = lote.Producto?.Nombre ?? string.Empty,
                ProductoCodigo = lote.Producto?.Codigo ?? string.Empty,
                ProductoCategoria = lote.Producto?.Categoria?.Nombre ?? "Sin categoría"
            };
        }
    }
}