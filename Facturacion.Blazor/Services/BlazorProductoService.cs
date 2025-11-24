using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace Facturacion.Blazor.Services
{
    public class BlazorProductoService : IProductoService
    {
        private readonly HttpClient _httpClient;

        public BlazorProductoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductoListaDto>> GetAllAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProductoListaDto>>("api/productos") ?? new List<ProductoListaDto>();
            }
            catch
            {
                return new List<ProductoListaDto>();
            }
        }

        public async Task<ProductoDetalleDto?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductoDetalleDto>($"api/productos/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> CrearProductoAsync(CrearProductoDto dto, int userId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/productos", dto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<int>();
                }
                throw new InvalidOperationException("Error al crear producto");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al crear producto: {ex.Message}");
            }
        }

        public async Task ActualizarProductoAsync(int id, ActualizarProductoDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/productos/{id}", dto);
                if (!response.IsSuccessStatusCode)
                {
                    throw new KeyNotFoundException("Producto no encontrado");
                }
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"Error al actualizar producto: {ex.Message}");
            }
        }

        public async Task DesactivarProductoAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/productos/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new KeyNotFoundException("Producto no encontrado");
                }
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"Error al desactivar producto: {ex.Message}");
            }
        }

        public async Task CrearLoteAsync(CrearLoteDto dto, int userId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/productos/lotes", dto);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException($"Error al crear lote: {error}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al crear lote: {ex.Message}");
            }
        }

        public async Task<List<CategoriaDto>> GetCategoriasAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CategoriaDto>>("api/productos/categorias") ?? new List<CategoriaDto>();
            }
            catch
            {
                return new List<CategoriaDto>();
            }
        }

        public async Task<List<ProductoComboDto>> GetComboProductosAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProductoComboDto>>("api/productos/combo") ?? new List<ProductoComboDto>();
            }
            catch
            {
                return new List<ProductoComboDto>();
            }
        }
    }
}