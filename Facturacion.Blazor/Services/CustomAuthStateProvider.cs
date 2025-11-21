using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks; // Asegurar que System.Threading.Tasks está incluido

namespace Facturacion.Blazor.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        // 1. Campo privado declarado y accesible en toda la clase
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            // Nota: Aquí no se valida la expiración del token, solo se lee. 
            // Esto es suficiente para el ámbito local de Blazor.
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        // 2. Método de notificación de inicio de sesión
        // Usamos async Task (en lugar de async void) por buenas prácticas.
        public async Task NotifyUserAuthentication(string token)
        {
            await _localStorage.SetItemAsync("authToken", token); // Uso de _localStorage correcto
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        // 3. Método de notificación de cierre de sesión
        // Usamos async Task (en lugar de async void).
        public async Task NotifyUserLogout()
        {
            await _localStorage.RemoveItemAsync("authToken"); // Uso de _localStorage correcto

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            );
        }
    }
}