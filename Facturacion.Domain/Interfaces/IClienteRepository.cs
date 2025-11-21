using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facturacion.Domain.Entities;

namespace Facturacion.Domain.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByIdentificacionAsync(string tipo, string numero);
    Task<List<Cliente>> GetAllActivosAsync();
    Task<List<Cliente>> BuscarAsync(string termino); // para modal de facturación
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    // No hay Delete, solo desactivar
    Task DesactivarAsync(int id);
}