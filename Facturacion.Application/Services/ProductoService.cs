using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;

namespace Facturacion.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repo;

    public ProductoService(IProductoRepository repo) => _repo = repo;

    public async Task<List<ProductoListaDto>> GetAllAsync()
    {
        var productos = await _repo.GetAllActivosConLotesAsync();

        return productos.Select(p => new ProductoListaDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Codigo = p.Codigo,
            Marca = p.Marca?.Nombre ?? "Sin marca",
            Modelo = p.Modelo,
            StockTotal = p.Lotes.Sum(l => l.Stock),
            PrecioVentaActual = p.Lotes
                .Where(l => l.Stock > 0)
                .OrderBy(l => l.FechaIngreso)
                .FirstOrDefault()?.PrecioVenta ?? 0m,
            Activo = p.Activo
        }).ToList();
    }

    public async Task<ProductoDetalleDto?> GetByIdAsync(int id)
    {
        var producto = await _repo.GetByIdConLotesAsync(id);
        if (producto == null) return null;

        var dto = new ProductoDetalleDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Codigo = producto.Codigo,
            CodigoBarra = producto.CodigoBarra,
            Marca = producto.Marca != null ? new MarcaDto { Id = producto.Marca.Id, Nombre = producto.Marca.Nombre } : null,
            Modelo = producto.Modelo,
            Categoria = producto.Categoria != null ? new CategoriaDto { Id = producto.Categoria.Id, Nombre = producto.Categoria.Nombre } : null,
            Descripcion = producto.Descripcion,
            Lotes = new List<ProductoLoteDto>()
        };

        foreach (var l in producto.Lotes.OrderBy(x => x.FechaIngreso))
        {
            dto.Lotes.Add(new ProductoLoteDto
            {
                Id = l.Id,
                Lote = l.Lote,
                PrecioCompra = l.PrecioCompra,
                PrecioVenta = l.PrecioVenta,
                Stock = l.Stock,
                FechaIngreso = l.FechaIngreso,
                FechaVencimiento = l.FechaVencimiento,
                Activo = l.Activo
            });
        }

        return dto;
    }

    public async Task<int> CrearProductoAsync(CrearProductoDto dto, int userId)
    {
        var existe = await _repo.GetByCodigoAsync(dto.Codigo);
        if (existe != null && existe.Activo)
            throw new InvalidOperationException("Ya existe un producto activo con ese código.");

        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Codigo = dto.Codigo,
            CodigoBarra = dto.CodigoBarra,
            Descripcion = dto.Descripcion,
            MarcaId = dto.MarcaId,
            CategoriaId = dto.CategoriaId,
            Modelo = dto.Modelo,
            CreatedBy = userId,
            Activo = true
        };

        await _repo.AddAsync(producto);
        return producto.Id;
    }

    public async Task ActualizarProductoAsync(int id, ActualizarProductoDto dto)
    {
        var producto = await _repo.GetByIdConLotesAsync(id)
            ?? throw new KeyNotFoundException("Producto no encontrado");

        producto.Nombre = dto.Nombre;
        producto.Codigo = dto.Codigo;
        producto.CodigoBarra = dto.CodigoBarra;
        producto.Descripcion = dto.Descripcion;
        producto.MarcaId = dto.MarcaId;
        producto.CategoriaId = dto.CategoriaId;
        producto.Modelo = dto.Modelo;

        await _repo.UpdateAsync(producto);
    }

    public async Task DesactivarProductoAsync(int id) => await _repo.DesactivarAsync(id);

    public async Task CrearLoteAsync(CrearLoteDto dto, int userId)
    {
        var producto = await _repo.GetByIdConLotesAsync(dto.ProductoId)
            ?? throw new KeyNotFoundException("Producto no encontrado");

        if (dto.PrecioVenta < dto.PrecioCompra)
            throw new InvalidOperationException("El precio de venta no puede ser menor al de compra.");

        if (dto.PrecioVenta > dto.PrecioCompra * 4)
            throw new InvalidOperationException("El precio de venta no puede superar 4 veces el costo.");

        var existeLote = await _repo.GetLoteAsync(dto.ProductoId, dto.Lote);
        if (existeLote != null)
            throw new InvalidOperationException($"El lote {dto.Lote} ya existe.");

        var lote = new ProductoLote
        {
            ProductoId = dto.ProductoId,
            Lote = dto.Lote,
            PrecioCompra = dto.PrecioCompra,
            PrecioVenta = dto.PrecioVenta,
            Stock = dto.Cantidad,
            FechaVencimiento = dto.FechaVencimiento,
            CreatedBy = userId
        };

        await _repo.AddLoteAsync(lote);
    }

    public async Task<List<CategoriaDto>> GetCategoriasAsync()
    {
        var categorias = await _repo.GetCategoriasAsync();
        return categorias.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nombre = c.Nombre
        }).ToList();
    }

    public async Task<List<ProductoComboDto>> GetComboProductosAsync()
    {
        var productos = await _repo.GetAllActivosConLotesAsync();
        
        return productos
            .Where(p => p.Activo)
            .Select(p => new ProductoComboDto
            {
                Id = p.Id,
                Codigo = p.Codigo ?? "SIN-CODIGO",
                Nombre = p.Nombre,
                Categoria = p.Categoria?.Nombre
            })
            .OrderBy(p => p.Nombre)
            .ToList();
    }
}