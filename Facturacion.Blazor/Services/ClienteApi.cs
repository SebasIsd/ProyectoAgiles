using System.Net.Http.Json;

namespace Facturacion.Blazor.Services;

public class ClienteApi
{
    private readonly HttpClient _http;
    public ClienteApi(IHttpClientFactory f) => _http = f.CreateClient("API");

    public Task<List<ClienteDto>?> ListarAsync() =>
        _http.GetFromJsonAsync<List<ClienteDto>>("/api/clientes");
}

public record ClienteDto(int Id, string Nombre, string RUC, string? Email);

