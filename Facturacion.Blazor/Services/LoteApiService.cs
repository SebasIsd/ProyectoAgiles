// Facturacion.Blazor/Services/LoteApiService.cs
using System.Net.Http.Json;
using Facturacion.Blazor.Models;

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
                var response = await _http.GetFromJsonAsync<List<LoteItem>>("api/lotes");
                return response ?? new List<LoteItem>();
            }
            catch
            {
                return new List<LoteItem>();
            }
        }

        public async Task<LoteItem?> GetByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<LoteItem>($"api/lotes/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateAsync(LoteItem lote)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/lotes", lote);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(LoteItem lote)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"api/lotes/{lote.LoteId}", lote);
                return response.IsSuccessStatusCode;
            }
            catch
            {
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
            catch
            {
                return false;
            }
        }

        public async Task<List<LoteItem>> SearchAsync(string searchTerm)
        {
            try
            {
                var url = $"api/lotes?search={Uri.EscapeDataString(searchTerm)}";
                var response = await _http.GetFromJsonAsync<List<LoteItem>>(url);
                return response ?? new List<LoteItem>();
            }
            catch
            {
                return new List<LoteItem>();
            }
        }
    }
}