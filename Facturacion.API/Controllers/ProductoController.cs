using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Facturacion.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _service;
    private readonly IHttpContextAccessor _http;

    public ProductosController(IProductoService service, IHttpContextAccessor http)
    {
        _service = service;
        _http = http;
    }

    private int UserId => int.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<ActionResult<List<ProductoListaDto>>> Get() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoDetalleDto>> Get(int id) =>
        await _service.GetByIdAsync(id) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    public async Task<ActionResult> Post(CrearProductoDto dto)
    {
        var id = await _service.CrearProductoAsync(dto, UserId);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, ActualizarProductoDto dto)
    {
        await _service.ActualizarProductoAsync(id, dto);
        return NoContent();
    }

    [HttpPost("lotes")]
    public async Task<ActionResult> PostLote(CrearLoteDto dto)
    {
        await _service.CrearLoteAsync(dto, UserId);
        return Ok(new { mensaje = "Lote creado correctamente" });
    }
    private readonly AppDbContext _context;
    // GET: api/productos/categorias
    [HttpGet("categorias")]
    public async Task<ActionResult<List<CategoriaDto>>> GetCategorias()
        => Ok(await _service.GetCategoriasAsync());

}