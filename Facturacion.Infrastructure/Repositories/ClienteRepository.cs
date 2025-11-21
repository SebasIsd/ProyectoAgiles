using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;
using Facturacion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Facturacion.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        }

        public async Task<Cliente?> GetByIdentificacionAsync(string tipoId, string numero)
        {
            var tipo = Enum.Parse<TipoIdentificacion>(tipoId.ToUpper());

            return await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    c.TipoIdentificacion == tipo &&
                    c.Identificacion == numero &&
                    c.Activo);
        }

        public async Task<List<Cliente>> GetAllActivosAsync()
        {
            return await _context.Clientes
                .Where(c => c.Activo)
                .ToListAsync();
        }

        public async Task<List<Cliente>> BuscarAsync(string termino)
        {
            return await _context.Clientes
                .Where(c => c.Activo && (
                    c.Nombre.Contains(termino) ||
                    c.Identificacion.Contains(termino) ||
                    (c.Email != null && c.Email.Contains(termino))
                ))
                .Take(50)                     // Aquí estaba el error
                .ToListAsync();
        }

        public async Task AddAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Activo = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}