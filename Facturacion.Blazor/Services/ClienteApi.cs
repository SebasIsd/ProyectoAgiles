// Facturacion.Blazor/Services/ClienteApi.cs
using Facturacion.Application.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Facturacion.Blazor.Services;

public class ClienteApi
{
    private readonly HttpClient _http;
    public ClienteApi(IHttpClientFactory f) => _http = f.CreateClient("API");

    public Task<List<ClienteDto>> ListarAsync() =>
        _http.GetFromJsonAsync<List<ClienteDto>>("api/clientes") ?? Task.FromResult(new List<ClienteDto>());

    public Task<ClienteDto?> PorIdAsync(int id) =>
        _http.GetFromJsonAsync<ClienteDto>($"api/clientes/{id}");

    public Task<List<ClienteDto>> BuscarAsync(string termino)
    {
        return _http.GetFromJsonAsync<List<ClienteDto>>($"api/clientes/buscar?termino={Uri.EscapeDataString(termino)}")
        ?? Task.FromResult(new List<ClienteDto>());
    }

    public Task<int> CrearAsync(CrearClienteDto dto) =>
        _http.PostAsJsonAsync("api/clientes", dto)
             .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<int>().Result);

    public Task ActualizarAsync(int id, ActualizarClienteDto dto) =>
        _http.PutAsJsonAsync($"api/clientes/{id}", dto);

    public Task DesactivarAsync(int id) =>
        _http.DeleteAsync($"api/clientes/{id}");

    public Task<List<ClienteDto>> GetClientesNuevosMesAsync() =>
        _http.GetFromJsonAsync<List<ClienteDto>>("api/clientes/clientes-nuevos-mes") ?? Task.FromResult(new List<ClienteDto>());

    public async Task<List<ClienteDto>> GetClientesRecientesAsync(int top)
    {
        var response = await _http.GetAsync($"api/clientes/recientes/{top}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ClienteDto>>() ?? new List<ClienteDto>();
    }
}