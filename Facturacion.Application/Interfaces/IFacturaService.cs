using Facturacion.Application.Dtos;
using Facturacion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Application.Interfaces;

public interface IFacturaService
{
    Task<List<FacturaDto>> GetAllAsync();
    Task<FacturaDto?> GetByIdAsync(int id);
    Task<FacturaDto> CrearFacturaAsync(CrearFacturaDto dto);
    Task ActualizarFacturaAsync(int id, ActualizarFacturaDto dto);
    Task EliminarFacturaAsync(int id);

    Task<List<FacturaDto>> GetFacturasMesActualAsync(); //
    Task<int> GetFacturasPendientesAsync(); //
    Task<List<decimal>> GetVentasUltimosMesesAsync(int meses); //
    Task<List<FacturaDto>> GetUltimasFacturasAsync(int top); //
}