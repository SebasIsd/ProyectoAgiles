// Controllers/FacturasController.cs
using Facturacion.Application.Dtos;
using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FacturasController : ControllerBase
{
    private readonly IFacturaService _facturaService;

    public FacturasController(IFacturaService facturaService)
    {
        _facturaService = facturaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _facturaService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var factura = await _facturaService.GetByIdAsync(id);
        return factura == null ? NotFound() : Ok(factura);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearFacturaDto dto)
    {
        var result = await _facturaService.CrearFacturaAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarFacturaDto dto)
    {
        await _facturaService.ActualizarFacturaAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _facturaService.EliminarFacturaAsync(id);
        return NoContent();
    }
}