using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Facturacion.Application.Services;

public class EmpresaService : IEmpresaService
{
    private readonly IEmpresaRepository _repo;

    public EmpresaService(IEmpresaRepository repo) => _repo = repo;

    public async Task<EmpresaDto?> GetActivaAsync()
    {
        var empresa = await _repo.GetActivaAsync();
        return empresa is null ? null : ToDto(empresa);
    }

    public async Task<EmpresaDto> CrearAsync(CrearEmpresaDto dto, int userId)
    {
        // Validar RUC
        if (dto.Ruc.Length != 13 || !long.TryParse(dto.Ruc, out _))
            throw new ArgumentException("RUC inválido");

        // Verificar si ya existe una empresa activa con el mismo RUC
        var existe = await _repo.GetAll().AnyAsync(e => e.Ruc == dto.Ruc && e.Activo);
        if (existe)
            throw new InvalidOperationException("Ya existe una empresa con este RUC.");

        var empresa = new Empresa
        {
            Ruc = dto.Ruc,
            RazonSocial = dto.RazonSocial,
            NombreComercial = dto.NombreComercial,
            DireccionMatriz = dto.DireccionMatriz,
            ContribuyenteEspecial = dto.ContribuyenteEspecial,
            ObligadoContabilidad = dto.ObligadoContabilidad,
            LogoPath = dto.LogoPath,
            CreatedBy = userId
        };

        await _repo.AddAsync(empresa);
        // SaveChanges ya se llama dentro de AddAsync

        return ToDto(empresa);
    }

    public async Task<EmpresaDto> ActualizarAsync(int id, ActualizarEmpresaDto dto, int userId)
    {
        var empresa = await _repo.GetByIdAsync(id);
        if (empresa == null) throw new KeyNotFoundException("Empresa no encontrada");

        empresa.RazonSocial = dto.RazonSocial;
        empresa.NombreComercial = dto.NombreComercial;
        empresa.DireccionMatriz = dto.DireccionMatriz;
        empresa.ContribuyenteEspecial = dto.ContribuyenteEspecial;
        empresa.ObligadoContabilidad = dto.ObligadoContabilidad;
        empresa.LogoPath = dto.LogoPath;
        empresa.Activo = dto.Activo;

        await _repo.UpdateAsync(empresa);
        // SaveChanges ya se llama dentro de UpdateAsync

        return ToDto(empresa);
    }

    public async Task<List<EmpresaDto>> GetAllAsync()
    {
        var empresas = await _repo.GetAll().ToListAsync();
        return empresas.Select(ToDto).ToList();
    }

    public async Task<EmpresaDto?> GetByIdAsync(int id)
    {
        var empresa = await _repo.GetByIdAsync(id);
        return empresa is null ? null : ToDto(empresa);
    }

    private static EmpresaDto ToDto(Empresa e) => new()
    {
        Id = e.Id,
        Ruc = e.Ruc,
        RazonSocial = e.RazonSocial,
        NombreComercial = e.NombreComercial,
        DireccionMatriz = e.DireccionMatriz,
        ContribuyenteEspecial = e.ContribuyenteEspecial,
        ObligadoContabilidad = e.ObligadoContabilidad,
        LogoPath = e.LogoPath,
        Activo = e.Activo
    };
}