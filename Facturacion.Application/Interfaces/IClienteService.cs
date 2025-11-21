using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facturacion.Application.DTOs;

namespace Facturacion.Application.Interfaces;

public interface IClienteService
{
    Task<List<ClienteDto>> GetAllActivosAsync();
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<ClienteDto?> GetByIdentificacionAsync(string tipo, string numero);
    Task<List<ClienteDto>> BuscarAsync(string termino);
    Task<int> CrearAsync(CrearClienteDto dto, int userId);
    Task ActualizarAsync(int id, ActualizarClienteDto dto, int userId);
    Task DesactivarAsync(int id);
}