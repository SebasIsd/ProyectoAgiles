using Facturacion.Application.DTOs;
using Facturacion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Application.Interfaces;

public interface IProductoService
{
    Task<List<ProductoListaDto>> GetAllAsync();
    Task<ProductoDetalleDto?> GetByIdAsync(int id);
    Task<int> CrearProductoAsync(CrearProductoDto dto, int userId);
    Task ActualizarProductoAsync(int id, ActualizarProductoDto dto);
    Task DesactivarProductoAsync(int id);
    Task CrearLoteAsync(CrearLoteDto dto, int userId);
    Task<List<CategoriaDto>> GetCategoriasAsync();
    
    // NUEVO MÉTODO PARA COMBO
    Task<List<ProductoComboDto>> GetComboProductosAsync();
}