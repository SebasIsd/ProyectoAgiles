using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facturacion.Domain.Entities;

namespace Facturacion.Domain.Interfaces;

public interface IEmpresaRepository
{
    // Métodos específicos
    Task<Empresa?> GetActivaAsync();

    // Métodos CRUD genéricos (simulados sin herencia)
    IQueryable<Empresa> GetAll();
    Task<Empresa?> GetByIdAsync(int id);
    Task AddAsync(Empresa entity);
    Task UpdateAsync(Empresa entity);
    Task SaveChangesAsync();
}