using Facturacion.Application.Dtos;
using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Facturacion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Facturacion.Application.Services;

public class FacturaService : IFacturaService
{
    private readonly IRepository<Factura> _facturaRepo;
    private readonly IRepository<FacturaDetalle> _detalleRepo;
    private readonly IRepository<ProductoLote> _loteRepo;
    private readonly IRepository<Secuencial> _secuencialRepo;
    private readonly IRepository<Cliente> _clienteRepo;

    public FacturaService(
        IRepository<Factura> facturaRepo,
        IRepository<FacturaDetalle> detalleRepo,
        IRepository<ProductoLote> loteRepo,
        IRepository<Secuencial> secuencialRepo,
        IRepository<Cliente> clienteRepo)
    {
        _facturaRepo = facturaRepo;
        _detalleRepo = detalleRepo;
        _loteRepo = loteRepo;
        _secuencialRepo = secuencialRepo;
        _clienteRepo = clienteRepo;
    }

    public async Task<List<FacturaDto>> GetAllAsync()
    {
        return await _facturaRepo.GetAll()
            .Include(f => f.Cliente)
            .Include(f => f.Detalles)
            .ThenInclude(d => d.Producto)
            .OrderByDescending(f => f.Id)
            .Select(f => new FacturaDto
            {
                Id = f.Id,
                Numero = f.Numero,
                FechaEmision = f.FechaEmision,
                ClienteNombre = f.Cliente.Nombre,
                Subtotal = f.Subtotal,
                Iva = f.Iva,
                Total = f.Total,
                Estado = f.Estado
            })
            .ToListAsync();
    }

    public async Task<FacturaDto?> GetByIdAsync(int id)
    {
        var factura = await _facturaRepo.GetAll()
            .Include(f => f.Cliente)
            .Include(f => f.Detalles)
            .ThenInclude(d => d.Producto)
            .Include(f => f.Detalles)
            .ThenInclude(d => d.Lote)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (factura == null) return null;

        return new FacturaDto
        {
            Id = factura.Id,
            Numero = factura.Numero,
            FechaEmision = factura.FechaEmision,
            ClienteNombre = factura.Cliente.Nombre,
            Subtotal = factura.Subtotal,
            Iva = factura.Iva,
            Total = factura.Total,
            Estado = factura.Estado,
            Detalles = factura.Detalles.Select(d => new FacturaDetalleDto
            {
                ProductoNombre = d.Producto.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                TotalLinea = d.TotalLinea
            }).ToList()
        };
    }

    public async Task<FacturaDto> CrearFacturaAsync(CrearFacturaDto dto)
    {
        // Validación básica
        if (dto.ClienteId <= 0) throw new Exception("Debe seleccionar un cliente");
        if (!dto.Detalles.Any() || dto.Detalles.All(d => d.ProductoId <= 0 || d.Cantidad <= 0))
            throw new Exception("Debe agregar al menos un producto válido");

        // Obtener y actualizar secuencial
        var secuencial = await _secuencialRepo.GetAll().FirstOrDefaultAsync()
            ?? throw new Exception("No se encontró configuración de secuencial");

        secuencial.SecuencialActual++;
        await _secuencialRepo.UpdateAsync(secuencial);

        var numeroFactura = $"001-001-{secuencial.SecuencialActual:000000000}";

        var factura = new Factura
        {
            Numero = numeroFactura,
            FechaEmision = DateTime.Today,
            ClienteId = dto.ClienteId,
            Estado = "Emitida",
            CreatedBy = 1 // Aquí pondrás el usuario autenticado después
        };

        decimal subtotalGeneral = 0m;

        foreach (var item in dto.Detalles)
        {
            // FIFO: lotes con stock, ordenados por fecha de ingreso (más antiguo primero)
            var lotesDisponibles = await _loteRepo.GetAll()
                .Where(l => l.ProductoId == item.ProductoId && l.Stock > 0 && l.Activo)
                .OrderBy(l => l.FechaIngreso)
                .ThenBy(l => l.Id)
                .ToListAsync();

            if (!lotesDisponibles.Any())
                throw new Exception($"No hay stock disponible para el producto ID {item.ProductoId}");

            int cantidadPendiente = item.Cantidad;

            foreach (var lote in lotesDisponibles)
            {
                if (cantidadPendiente <= 0) break;

                int cantidadATomar = Math.Min(cantidadPendiente, lote.Stock);

                // Descontar stock
                lote.Stock -= cantidadATomar;
                await _loteRepo.UpdateAsync(lote);

                var detalle = new FacturaDetalle
                {
                    ProductoId = item.ProductoId,
                    ProductoLoteId = lote.Id,
                    Cantidad = cantidadATomar,
                    PrecioUnitario = lote.PrecioVenta,
                    SubtotalLinea = cantidadATomar * lote.PrecioVenta,
                    ValorIva = Math.Round(cantidadATomar * lote.PrecioVenta * 0.15m, 2),
                    TotalLinea = cantidadATomar * lote.PrecioVenta * 1.15m
                };

                factura.Detalles.Add(detalle);
                subtotalGeneral += detalle.SubtotalLinea;
                cantidadPendiente -= cantidadATomar;
            }

            if (cantidadPendiente > 0)
                throw new Exception($"Stock insuficiente para el producto ID {item.ProductoId}");
        }

        factura.Subtotal = subtotalGeneral;
        factura.Iva = Math.Round(subtotalGeneral * 0.15m, 2);
        factura.Total = subtotalGeneral + factura.Iva;

        await _facturaRepo.AddAsync(factura);
        await _facturaRepo.SaveChangesAsync();

        var cliente = await _clienteRepo.GetByIdAsync(dto.ClienteId);

        return new FacturaDto
        {
            Id = factura.Id,
            Numero = factura.Numero,
            FechaEmision = factura.FechaEmision,
            ClienteNombre = cliente?.Nombre ?? "Desconocido",
            Subtotal = factura.Subtotal,
            Iva = factura.Iva,
            Total = factura.Total,
            Estado = factura.Estado
        };
    }

    public async Task ActualizarFacturaAsync(int id, ActualizarFacturaDto dto)
    {
        // Por ahora solo permitimos editar si está en Borrador
        var factura = await _facturaRepo.GetByIdAsync(id);
        if (factura == null) throw new Exception("Factura no encontrada");
        if (factura.Estado != "Borrador") throw new Exception("Solo se pueden editar facturas en estado Borrador");

        // Aquí iría lógica similar a crear, pero con actualización
        // Por ahora lo dejamos simple
        throw new NotImplementedException("Edición pendiente para próxima fase");
    }

    public async Task EliminarFacturaAsync(int id)
    {
        var factura = await _facturaRepo.GetByIdAsync(id);
        if (factura == null) throw new Exception("Factura no encontrada");
        if (factura.Estado != "Borrador") throw new Exception("Solo se pueden eliminar facturas en Borrador");

        await _facturaRepo.DeleteAsync(factura);
        await _facturaRepo.SaveChangesAsync();
    }
}