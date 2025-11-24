using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Facturacion.Domain;
using Facturacion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Facturacion.Application.Services
{
    public class CobroService : ICobroService
    {
        private readonly IRepository<Cobro> _cobroRepo;
        private readonly IRepository<Factura> _facturaRepo;

        public CobroService(
            IRepository<Cobro> cobroRepo,
            IRepository<Factura> facturaRepo)
        {
            _cobroRepo = cobroRepo;
            _facturaRepo = facturaRepo;
        }

        // ---------------------------------------------------------------------
        // GET ALL
        // ---------------------------------------------------------------------
        public async Task<IEnumerable<CobroDto>> GetAll()
        {
            return await _cobroRepo.GetAll()
                .Include(c => c.Factura)
                .OrderByDescending(c => c.Id)
                .Select(c => new CobroDto
                {
                    Id = c.Id,
                    FacturaId = c.FacturaId,
                    Monto = c.Monto,
                    MetodoPago = c.MetodoPago,
                    Fecha = c.Fecha
                })
                .ToListAsync();
        }

        // ---------------------------------------------------------------------
        // GET BY ID
        // ---------------------------------------------------------------------
        public async Task<CobroDto?> GetById(int id)
        {
            var cobro = await _cobroRepo.GetAll()
                .Include(c => c.Factura)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cobro == null) return null;

            return new CobroDto
            {
                Id = cobro.Id,
                FacturaId = cobro.FacturaId,
                Monto = cobro.Monto,
                MetodoPago = cobro.MetodoPago,
                Fecha = cobro.Fecha
            };
        }

        // ---------------------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------------------
        public async Task<CobroDto> Create(CrearCobroDto dto)
        {
            // Validar factura existente
            var factura = await _facturaRepo.GetByIdAsync(dto.FacturaId)
                ?? throw new Exception("Factura no encontrada");

            if (dto.Monto <= 0)
                throw new Exception("El monto debe ser mayor a cero");

            // Total ya cobrado
            var totalCobrado = await _cobroRepo.GetAll()
                .Where(c => c.FacturaId == dto.FacturaId)
                .SumAsync(c => c.Monto);

            if (totalCobrado + dto.Monto > factura.Total)
                throw new Exception("El monto excede el saldo de la factura");

            // Crear cobro
            var cobro = new Cobro
            {
                FacturaId = dto.FacturaId,
                Monto = dto.Monto,
                MetodoPago = dto.MetodoPago,
                Fecha = DateTime.Now
            };

            await _cobroRepo.AddAsync(cobro);
            await _cobroRepo.SaveChangesAsync();

            // Actualizar estado de factura
            totalCobrado += dto.Monto;

            if (totalCobrado == factura.Total)
            {
                factura.Estado = "Pagada";
            }
            else
            {
                factura.Estado = "Emitida";
            }

            await _facturaRepo.UpdateAsync(factura);
            await _facturaRepo.SaveChangesAsync();

            return new CobroDto
            {
                Id = cobro.Id,
                FacturaId = cobro.FacturaId,
                Monto = cobro.Monto,
                MetodoPago = cobro.MetodoPago,
                Fecha = cobro.Fecha
            };
        }

        // ---------------------------------------------------------------------
        // DELETE
        // ---------------------------------------------------------------------
        public async Task<bool> Delete(int id)
        {
            var cobro = await _cobroRepo.GetByIdAsync(id)
                ?? throw new Exception("Cobro no encontrado");

            var factura = await _facturaRepo.GetByIdAsync(cobro.FacturaId)
                ?? throw new Exception("Factura asociada no encontrada");

            await _cobroRepo.DeleteAsync(cobro);
            await _cobroRepo.SaveChangesAsync();

            // Recalcular cobros
            var totalCobrado = await _cobroRepo.GetAll()
                .Where(c => c.FacturaId == factura.Id)
                .SumAsync(c => c.Monto);

            factura.Estado = totalCobrado >= factura.Total ? "Pagada" : "Emitida";

            await _facturaRepo.UpdateAsync(factura);
            await _facturaRepo.SaveChangesAsync();

            return true;
        }
    }
}
