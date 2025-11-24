using System.Net.Http.Json;
using Facturacion.Application.DTOs;

namespace Facturacion.Blazor.Services
{
    public class CobroService
    {
        private readonly HttpClient _http;

        public CobroService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CobroDto>> GetAll()
        {
            return await _http.GetFromJsonAsync<List<CobroDto>>("api/cobro")
                   ?? new List<CobroDto>();
        }

        public async Task<CobroDto?> Get(int id)
        {
            return await _http.GetFromJsonAsync<CobroDto>($"api/cobro/{id}");
        }

        public async Task<CobroDto> Create(CrearCobroDto dto)
        {
            var res = await _http.PostAsJsonAsync("api/cobro", dto);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<CobroDto>();
        }

        public async Task<bool> Delete(int id)
        {
            var res = await _http.DeleteAsync($"api/cobro/{id}");
            return res.IsSuccessStatusCode;
        }
    }
}
