// Facturacion.Blazor/Services/LoteApiService.cs
using System.Net.Http.Json;
using Facturacion.Blazor.Models;
using Facturacion.Application.DTOs;

namespace Facturacion.Blazor.Services
{
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
                Console.WriteLine("🔍 Llamando a: api/lotes");
                
                // 1. Verificar la respuesta HTTP
                var httpResponse = await _http.GetAsync("api/lotes");
                Console.WriteLine($"🔍 Status Code: {httpResponse.StatusCode}");
                
                if (!httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ ERROR HTTP: {httpResponse.StatusCode}");
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error content: {errorContent}");
                    return new List<LoteItem>();
                }

                // 2. Deserializar
                var response = await httpResponse.Content.ReadFromJsonAsync<List<LoteDto>>();
                Console.WriteLine($"🔍 Lotes DTO recibidos: {response?.Count ?? 0}");

                // 3. Mostrar detalles de los primeros lotes
                if (response != null && response.Any())
                {
                    Console.WriteLine("🔍 Detalles de los lotes:");
                    foreach (var lote in response.Take(3))
                    {
                        Console.WriteLine($"  📦 Lote {lote.Id}: '{lote.Lote}'");
                        Console.WriteLine($"     Producto: {lote.ProductoNombre} (ID: {lote.ProductoId})");
                        Console.WriteLine($"     Precio: C{lote.PrecioCompra} -> V{lote.PrecioVenta}");
                        Console.WriteLine($"     Stock: {lote.Stock}, Fecha: {lote.FechaIngreso:dd/MM/yyyy}");
                        Console.WriteLine($"     Activo: {lote.Activo}");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️  NO HAY LOTES en la respuesta del backend");
                    return new List<LoteItem>(); // ⭐ SOLO lista vacía
                }

                // 4. Mapear
                var lotesMapeados = response.Select(MapToLoteItem).ToList();
                Console.WriteLine($"✅ Lotes mapeados: {lotesMapeados.Count}");
                
                return lotesMapeados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR en GetAllAsync: {ex.Message}");
                return new List<LoteItem>(); // ⭐ SOLO lista vacía, NO datos de prueba
            }
        }

        public async Task<LoteItem?> GetByIdAsync(int id)
        {
            try
            {
                Console.WriteLine($"🔍 GetByIdAsync llamado para ID: {id}");
                var response = await _http.GetFromJsonAsync<LoteDto>($"api/lotes/{id}");
                
                if (response == null)
                {
                    Console.WriteLine($"⚠️  No se encontró lote con ID: {id}");
                    return null;
                }
                
                Console.WriteLine($"✅ Lote encontrado: {response.Lote}");
                return MapToLoteItem(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(LoteItem lote)
        {
            try
            {
                Console.WriteLine($"🔍 CreateAsync llamado para lote: {lote.Lote}");
                
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
                var success = response.IsSuccessStatusCode;
                
                Console.WriteLine($"📝 CreateAsync resultado: {success}");
                if (!success)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error del servidor: {error}");
                }
                
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR en CreateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(LoteItem lote)
        {
            try
            {
                Console.WriteLine($"🔍 UpdateAsync llamado para lote ID: {lote.Id}");
                
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
                var success = response.IsSuccessStatusCode;
                
                Console.WriteLine($"📝 UpdateAsync resultado: {success}");
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR en UpdateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                Console.WriteLine($"🔍 DeleteAsync llamado para ID: {id}");
                var response = await _http.DeleteAsync($"api/lotes/{id}");
                var success = response.IsSuccessStatusCode;
                
                Console.WriteLine($"📝 DeleteAsync resultado: {success}");
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR en DeleteAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<LoteItem>> SearchAsync(string searchTerm)
        {
            try
            {
                Console.WriteLine($"🔍 SearchAsync llamado con: '{searchTerm}'");
                var url = $"api/lotes?search={Uri.EscapeDataString(searchTerm)}";
                var response = await _http.GetFromJsonAsync<List<LoteDto>>(url);
                
                Console.WriteLine($"🔍 SearchAsync resultados: {response?.Count ?? 0}");
                return response?.Select(MapToLoteItem).ToList() ?? new List<LoteItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR en SearchAsync: {ex.Message}");
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