// Facturacion.Blazor/Services/ProductoApiService.cs
using System.Net.Http.Json;
using Facturacion.Blazor.Models;

namespace Facturacion.Blazor.Services
{
    public interface IProductoApiService
    {
        Task<List<ProductoCombo>> GetAllAsync();
    }

    public class ProductoApiService : IProductoApiService
    {
        private readonly HttpClient _http;
        
        public ProductoApiService(HttpClient http) => _http = http;
        
        public async Task<List<ProductoCombo>> GetAllAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<ProductoCombo>>("api/productos/combo");
                return response ?? new List<ProductoCombo>();
            }
            catch
            {
                return new List<ProductoCombo>();
            }
        }
    }
}