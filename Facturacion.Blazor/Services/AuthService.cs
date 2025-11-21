// Facturacion.Blazor/Services/AuthService.cs

using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
// ELIMINAR O COMENTAR: using Facturacion.Blazor.Providers; // Esto causaba el error de compilación

namespace Facturacion.Blazor.Services
{
    public class AuthService
    {
        private readonly IHttpClientFactory _httpFactory;
        // Definir directamente el tipo concreto si se inyecta AuthenticationStateProvider y se usa el cast:
        private readonly CustomAuthStateProvider _authProvider; 

        // El constructor necesita el cast a CustomAuthStateProvider
        public AuthService(IHttpClientFactory httpFactory, AuthenticationStateProvider authProvider)
        {
            _httpFactory = httpFactory;
            // Casting a CustomAuthStateProvider para usar sus métodos de notificación
            _authProvider = (CustomAuthStateProvider)authProvider; 
        }

        // --- Modelos ---
        public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
        
        // La respuesta del API ahora es solo el token, usando minúsculas.
        public class LoginResponse { public string token { get; set; } = ""; } 
        
        public class LoginResult
        {
            public bool ok { get; set; }
            public string? error { get; set; }
        }
        // -----------------------------

        // Renombramos 'correo' a 'email'
        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                var client = _httpFactory.CreateClient("API");
                
                // Los datos enviados coinciden con el LoginDto (Email y Password) de la API
                var resp = await client.PostAsJsonAsync("api/Auth/login", 
                    new LoginRequest { Email = email, Password = password }
                );

                if (!resp.IsSuccessStatusCode)
                {
                    var msg = await resp.Content.ReadAsStringAsync();
                    // Devuelve el mensaje de error de la API (ej: "Credenciales incorrectas")
                    return new LoginResult { ok = false, error = string.IsNullOrWhiteSpace(msg) ? resp.ReasonPhrase : msg };
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dto = await resp.Content.ReadFromJsonAsync<LoginResponse>(options);
                
                if (dto is null || string.IsNullOrWhiteSpace(dto.token))
                    return new LoginResult { ok = false, error = "Respuesta inválida del servidor: Falta el token." };

                
                // Almacenar el token y notificar el cambio de autenticación
                await _authProvider.NotifyUserAuthentication(dto.token);
                

                return new LoginResult { ok = true }; 
            }
            catch (HttpRequestException ex)
            {
                return new LoginResult { ok = false, error = $"No se pudo contactar la API. Verifica que la API (puerto 5183) esté corriendo. Detalle: {ex.Message}" };
            }
            catch (Exception ex)
            {
                return new LoginResult { ok = false, error = $"Error inesperado: {ex.Message}" };
            }
        }

        public async Task LogoutAsync()
        {
            await _authProvider.NotifyUserLogout();
        }
    }
}