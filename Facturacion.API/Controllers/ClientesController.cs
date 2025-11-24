using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Facturacion.Application.Services;
using Facturacion.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;

    public ClientesController(IClienteService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ClienteDto>>> GetAll()
        => Ok(await _service.GetAllActivosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDto>> Get(int id)
    {
        var cliente = await _service.GetByIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<List<ClienteDto>>> Buscar([FromQuery] string termino)
        => Ok(await _service.BuscarAsync(termino));

    [HttpGet("por-identificacion")]
    public async Task<ActionResult<ClienteDto>> GetByIdentificacion(
        [FromQuery] string tipo, [FromQuery] string numero)
    {
        var cliente = await _service.GetByIdentificacionAsync(tipo, numero);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
      public async Task<ActionResult<int>> Crear(CrearClienteDto dto)
    {
        var userId = 1; // TODO: obtener del token cuando tengas auth
        var id = await _service.CrearAsync(dto, userId);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, ActualizarClienteDto dto)
    {
        var userId = 1;
        await _service.ActualizarAsync(id, dto, userId);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        await _service.DesactivarAsync(id);
        return NoContent();
    }

    [HttpGet("clientes-nuevos-mes")]
    public async Task<ActionResult<List<ClienteDto>>> GetClientesNuevosMes()
        => Ok(await _service.GetClientesNuevosMesAsync());

    [HttpGet("recientes/{top}")]
    public async Task<ActionResult<List<ClienteDto>>> GetClientesRecientes(int top)
    {
        var clientes = await _service.GetClientesRecientesAsync(top);
        return Ok(clientes);
    }
}