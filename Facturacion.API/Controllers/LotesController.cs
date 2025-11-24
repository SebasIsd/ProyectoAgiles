// Facturacion.API/Controllers/LotesController.cs
using Microsoft.AspNetCore.Mvc;
using Facturacion.Application.DTOs;
using Facturacion.Application.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facturacion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotesController : ControllerBase
    {
        private readonly ILoteService _loteService;

        public LotesController(ILoteService loteService)
        {
            _loteService = loteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoteDto>>> GetLotes([FromQuery] string? search = null)
        {
            try
            {
                IEnumerable<LoteDto> lotes;
                if (!string.IsNullOrEmpty(search))
                {
                    lotes = await _loteService.SearchAsync(search);
                }
                else
                {
                    lotes = await _loteService.GetAllAsync();
                }
                return Ok(lotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoteDto>> GetLote(int id)
        {
            try
            {
                var lote = await _loteService.GetByIdAsync(id);
                if (lote == null)
                    return NotFound();

                return Ok(lote);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<LoteDto>> CreateLote(CreateLoteDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nuevoLote = await _loteService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetLote), new { id = nuevoLote.Id }, nuevoLote);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el lote: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LoteDto>> UpdateLote(int id, UpdateLoteDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID no coincide");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var loteActualizado = await _loteService.UpdateAsync(updateDto);
                return Ok(loteActualizado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el lote: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLote(int id)
        {
            try
            {
                var resultado = await _loteService.DeleteAsync(id);
                if (!resultado)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}