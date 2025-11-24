using Blazored.LocalStorage;
using CurrieTechnologies.Razor.SweetAlert2;
using Facturacion.Blazor.Components;
using Facturacion.Blazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Componentes raíz
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Servicios base
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

// 1. URL de la API (Asegurando que sea http:// si la API no usa HTTPS)
var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5183";

// 2. Handler que agrega el Bearer desde LocalStorage (Necesario)
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

// 3. Autenticación personalizada (Deben ir juntos)
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ProductoApi>();     // <--- ¡ESTO ES LO QUE FALTA!
builder.Services.AddScoped<AuthService>();     // Probablemente también necesites este
builder.Services.AddScoped<FacturaApiService>();
builder.Services.AddSweetAlert2();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<LoteItem>();
builder.Services.AddScoped<LoteState>();

// 4. Configuración del CLIENTE NOMBRADO "API"
// Este registro es usado por IHttpClientFactory en AuthService.cs.
builder.Services.AddHttpClient("API", client =>
{
    // Esto configura el BaseAddress para el cliente "API"
    client.BaseAddress = new Uri(apiBase);
})
// Añadimos el handler, ya que es el cliente que gestionará todas las llamadas autenticadas.
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();


// 5. REGISTRO DEL CLIENTE POR DEFECTO (LÍNEA CLAVE A RE-INCLUIR)
// Esta línea le dice al DI que cuando un servicio pida 'HttpClient' (sin fábrica),
// debe darle una instancia del cliente nombrado "API".
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient("API"));


// 6. Registramos los servicios que usan HttpClient, como ClienteApi
builder.Services.AddScoped<ClienteApi>();



// 👉 agrega esto:
builder.Services.AddSingleton<FacturaState>();

await builder.Build().RunAsync();