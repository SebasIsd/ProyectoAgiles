using System.Net.Http.Json;
using Facturacion.Blazor.Models;

namespace Facturacion.Blazor.Services
{
    public class FacturaApi
    {
        private readonly HttpClient _http;

        public FacturaApi(HttpClient http)
        {
            _http = http;
        }

        // ============ CLIENTES ============
        public async Task<List<Cliente>> GetClientes()
        {
            var result = await _http.GetFromJsonAsync<List<Cliente>>("api/clientes");
            return result ?? new();
        }

        // ============ LOTES DE PRODUCTOS ============
        public async Task<List<LoteVm>> GetLotes()
        {
            var result = await _http.GetFromJsonAsync<List<LoteVm>>("api/lotes");
            return result ?? new();
        }

        // ============ CREAR FACTURA ============
        public async Task<bool> Create(FacturaVm factura)
        {
            var res = await _http.PostAsJsonAsync("api/facturas", factura);
            return res.IsSuccessStatusCode;
        }
    }
}
