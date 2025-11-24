using Blazored.LocalStorage;
using CurrieTechnologies.Razor.SweetAlert2;
using Facturacion.Blazor.Components;
using Facturacion.Blazor.Services;
using Facturacion.Application.Interfaces;
using Facturacion.Application.Services;
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

// 1. URL de la API
var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5183";

// 2. Handler que agrega el Bearer desde LocalStorage
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

// 3. Autenticación personalizada
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// ===================================================
// SERVICIOS DE APPLICATION - TODOS REGISTRADOS
// ===================================================
builder.Services.AddScoped<IProductoService, BlazorProductoService>();

builder.Services.AddScoped<ILoteService, LoteService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();

// ===================================================
// SERVICIOS DE API (HTTP CLIENT)
// ===================================================
builder.Services.AddScoped<ProductoApi>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ILoteApiService, LoteApiService>();
builder.Services.AddScoped<LoteState>();
builder.Services.AddScoped<IProductoApiService, ProductoApiService>();
builder.Services.AddScoped<FacturaApiService>();
builder.Services.AddScoped<ClienteApi>();
builder.Services.AddSingleton<FacturaState>();

builder.Services.AddSweetAlert2();
builder.Services.AddScoped<AlertService>();

// 4. Configuración del CLIENTE NOMBRADO "API"
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(apiBase);
})
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

// 5. REGISTRO DEL CLIENTE POR DEFECTO
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient("API"));

await builder.Build().RunAsync();