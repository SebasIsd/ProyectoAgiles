using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;
using Facturacion.Infrastructure.Persistence;
using Facturacion.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Facturacion.Infrastructure.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    private readonly AppDbContext _context;

    public EmpresaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Empresa?> GetActivaAsync()
    {
        return await _context.Set<Empresa>()
            .Where(e => e.Activo)
            .OrderByDescending(e => e.Id)
            .FirstOrDefaultAsync();
    }

    // Métodos CRUD genéricos (simulados sin herencia)
    public IQueryable<Empresa> GetAll()
    {
        return _context.Set<Empresa>();
    }

    public async Task<Empresa?> GetByIdAsync(int id)
    {
        return await _context.Set<Empresa>().FindAsync(id);
    }

    public async Task AddAsync(Empresa entity)
    {
        _context.Set<Empresa>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Empresa entity)
    {
        _context.Set<Empresa>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}