using Microsoft.AspNetCore.Mvc;
using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;     

[Route("api/[controller]")]
[ApiController]
public class FacturaController : ControllerBase
{
    private readonly IFacturaService _service;

    public FacturaController(IFacturaService service) => _service = service;

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FacturaDto dto)
        => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FacturaDto dto)
    {
        dto.Id = id;
        await _service.UpdateAsync(dto);
        return Ok();
    }
}
