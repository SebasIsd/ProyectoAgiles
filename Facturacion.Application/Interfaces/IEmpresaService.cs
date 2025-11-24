using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facturacion.Application.DTOs;

namespace Facturacion.Application.Interfaces;

public interface IEmpresaService
{
    Task<EmpresaDto?> GetActivaAsync();
    Task<EmpresaDto> CrearAsync(CrearEmpresaDto dto, int userId);
    Task<EmpresaDto> ActualizarAsync(int id, ActualizarEmpresaDto dto, int userId);
    Task<List<EmpresaDto>> GetAllAsync();
    Task<EmpresaDto?> GetByIdAsync(int id);
}