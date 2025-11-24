using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CobroController : ControllerBase
    {
        private readonly ICobroService _service;

        public CobroController(ICobroService service)
        {
            _service = service;
        }

        // ---------------------------------------------------------------------
        // GET ALL
        // ---------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // ---------------------------------------------------------------------
        // GET BY ID
        // ---------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _service.GetById(id);
            if (c == null) 
                return NotFound();

            return Ok(c);
        }

        // ---------------------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearCobroDto dto)
        {
            try
            {
                var c = await _service.Create(dto);
                return CreatedAtAction(nameof(Get), new { id = c.Id }, c);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------------
        // DELETE
        // ---------------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.Delete(id);
                if (!ok) 
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
