// Facturacion.Blazor/Services/LoteApiService.cs
using System.Net.Http.Json;
using Facturacion.Blazor.Models;
using Facturacion.Application.DTOs;

namespace Facturacion.Blazor.Services
{
    // ⭐⭐ INTERFAZ QUE FALTABA ⭐⭐
    public interface ILoteApiService
    {
        Task<List<LoteItem>> GetAllAsync();
        Task<LoteItem?> GetByIdAsync(int id);
        Task<bool> CreateAsync(LoteItem lote);
        Task<bool> UpdateAsync(LoteItem lote);
        Task<bool> DeleteAsync(int id);
        Task<List<LoteItem>> SearchAsync(string searchTerm);
    }

    public class LoteApiService : ILoteApiService
    {
        private readonly HttpClient _http;

        public LoteApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<LoteItem>> GetAllAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<LoteDto>>("api/lotes");
                var lotesMapeados = response?.Select(MapToLoteItem).ToList() 
                                 ?? new List<LoteItem>();
                
                Console.WriteLine($"⭐ Lotes mapeados: {lotesMapeados.Count}");
                return lotesMapeados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⭐ ERROR en GetAllAsync: {ex.Message}");
                return new List<LoteItem>();
            }
        }

        public async Task<LoteItem?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<LoteDto>($"api/lotes/{id}");
                return response != null ? MapToLoteItem(response) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⭐ ERROR en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(LoteItem lote)
        {
            try
            {
                var createDto = new CreateLoteDto
                {
                    ProductoId = lote.ProductoId,
                    Lote = lote.Lote,
                    PrecioCompra = lote.PrecioCompra,
                    PrecioVenta = lote.PrecioVenta,
                    Stock = lote.Stock,
                    FechaIngreso = lote.FechaIngreso,
                    FechaVencimiento = lote.FechaVencimiento,
                    CreatedBy = 1
                };

                var response = await _http.PostAsJsonAsync("api/lotes", createDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⭐ ERROR en CreateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(LoteItem lote)
        {
            try
            {
                var updateDto = new UpdateLoteDto
                {
                    Id = lote.Id,
                    Lote = lote.Lote,
                    PrecioCompra = lote.PrecioCompra,
                    PrecioVenta = lote.PrecioVenta,
                    Stock = lote.Stock,
                    FechaIngreso = lote.FechaIngreso,
                    FechaVencimiento = lote.FechaVencimiento,
                    Activo = lote.Activo
                };

                var response = await _http.PutAsJsonAsync($"api/lotes/{lote.Id}", updateDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⭐ ERROR en UpdateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/lotes/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⭐ ERROR en DeleteAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<LoteItem>> SearchAsync(string searchTerm)
        {
            try
            {
                var url = $"api/lotes?search={Uri.EscapeDataString(searchTerm)}";
                var response = await _http.GetFromJsonAsync<List<LoteDto>>(url);
                return response?.Select(MapToLoteItem).ToList() ?? new List<LoteItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⭐ ERROR en SearchAsync: {ex.Message}");
                return new List<LoteItem>();
            }
        }

        private LoteItem MapToLoteItem(LoteDto dto)
        {
            return new LoteItem
            {
                Id = dto.Id,
                ProductoId = dto.ProductoId,
                Lote = dto.Lote,
                PrecioCompra = dto.PrecioCompra,
                PrecioVenta = dto.PrecioVenta,
                Stock = dto.Stock,
                FechaIngreso = dto.FechaIngreso,
                FechaVencimiento = dto.FechaVencimiento,
                Activo = dto.Activo,
                ProductoNombre = dto.ProductoNombre ?? "N/A",
                ProductoCodigo = dto.ProductoCodigo ?? "N/A", 
                ProductoCategoria = dto.ProductoCategoria ?? "N/A"
            };
        }
    }
}