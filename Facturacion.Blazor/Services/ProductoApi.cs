using Facturacion.Application.DTOs;
using System.Net.Http.Json;

namespace Facturacion.Blazor.Services;

public class ProductoApi
{
    private readonly HttpClient _http;
    public ProductoApi(IHttpClientFactory f) => _http = f.CreateClient("API");

    public Task<List<ProductoListaDto>?> ListarAsync() =>
        _http.GetFromJsonAsync<List<ProductoListaDto>>("api/productos");

    public Task<ProductoDetalleDto?> GetDetalleAsync(int id) =>
        _http.GetFromJsonAsync<ProductoDetalleDto>($"api/productos/{id}");

    public async Task Crear(CrearProductoDto dto)
{
    var resp = await _http.PostAsJsonAsync("api/productos", dto);

    if (!resp.IsSuccessStatusCode)
    {
        var error = await resp.Content.ReadAsStringAsync();
        throw new Exception(error);
    }
}

public async Task Actualizar(int id, ActualizarProductoDto dto)
{
    var resp = await _http.PutAsJsonAsync($"api/productos/{id}", dto);

    if (!resp.IsSuccessStatusCode)
    {
        var error = await resp.Content.ReadAsStringAsync();
        throw new Exception(error);
    }
}

    public Task CrearLote(CrearLoteDto dto) =>
        _http.PostAsJsonAsync("api/productos/lotes", dto).ContinueWith(_ => { });

    public async Task<List<CategoriaDto>> GetCategoriasAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<CategoriaDto>>("api/productos/categorias") ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<MarcaDto>> GetMarcasAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<MarcaDto>>("api/productos/marcas") ?? new();
        }
        catch
        {
            return new();
        }
    }
}