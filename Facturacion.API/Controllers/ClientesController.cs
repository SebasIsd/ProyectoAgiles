using Facturacion.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController(AppDbContext db) : ControllerBase
{
    // GET /api/clientes
    [HttpGet] // sin [Authorize] por ahora
    public IActionResult Get()
    {
        var data = db.Clientes
            .Select(c => new { c.Id, c.Nombre, c.RUC, c.Email })
            .ToList();
        return Ok(data);
    }
}
