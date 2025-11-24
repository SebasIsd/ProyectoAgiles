using Facturacion.Application.Dtos;
using Facturacion.Application.DTOs;
using System.Net.Http.Json;

namespace Facturacion.Blazor.Services
{
    public class FacturaApiService
    {
        private readonly HttpClient _http;

        public FacturaApiService(HttpClient http) => _http = http;

        public async Task<List<FacturaDto>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<FacturaDto>>("api/facturas") ?? new();

        public async Task<FacturaDto?> GetByIdAsync(int id) =>
            await _http.GetFromJsonAsync<FacturaDto>($"api/facturas/{id}");

        public async Task CrearAsync(CrearFacturaDto dto) =>
            await _http.PostAsJsonAsync("api/facturas", dto);

        // En Services/FacturaApiService.cs
        public async Task EliminarAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/facturas/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task ActualizarAsync(int id, ActualizarFacturaDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/facturas/{id}", dto);
            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());
        }
        public async Task<List<FacturaDto>> GetFacturasMesActualAsync() =>
        await _http.GetFromJsonAsync<List<FacturaDto>>("api/facturas/mes-actual") ?? new();

        public async Task<int> GetFacturasPendientesAsync() =>
            await _http.GetFromJsonAsync<int>("api/facturas/pendientes-count");

        public async Task<List<decimal>> GetVentasUltimosMesesAsync(int meses) =>
            await _http.GetFromJsonAsync<List<decimal>>($"api/facturas/ventas-ultimos-meses/{meses}") ?? new();

        public async Task<List<FacturaDto>> GetUltimasFacturasAsync(int top) =>
            await _http.GetFromJsonAsync<List<FacturaDto>>($"api/facturas/ultimas/{top}") ?? new();
    }
}
