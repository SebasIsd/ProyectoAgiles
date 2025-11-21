using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facturacion.Application.DTOs;
using Facturacion.Application.Interfaces;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Interfaces;

namespace Facturacion.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repo;

    public ClienteService(IClienteRepository repo) => _repo = repo;

    public async Task<List<ClienteDto>> GetAllActivosAsync()
    {
        var clientes = await _repo.GetAllActivosAsync();
        return clientes.Select(ToDto).ToList();
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        return c is null ? null : ToDto(c);
    }

    public async Task<ClienteDto?> GetByIdentificacionAsync(string tipo, string numero)
    {
        var c = await _repo.GetByIdentificacionAsync(tipo, numero);
        return c is null ? null : ToDto(c);
    }

    public async Task<List<ClienteDto>> BuscarAsync(string termino)
    {
        var clientes = await _repo.BuscarAsync(termino);
        return clientes.Select(ToDto).ToList();
    }

    public async Task<int> CrearAsync(CrearClienteDto dto, int userId)
    {
        ValidarIdentificacion(dto.TipoIdentificacion, dto.Identificacion);

        var existe = await _repo.GetByIdentificacionAsync(dto.TipoIdentificacion, dto.Identificacion);
        if (existe != null)
            throw new InvalidOperationException("Ya existe un cliente con esa identificación.");

        var cliente = new Cliente
        {
            Nombre = dto.Nombre.Trim(),
            TipoIdentificacion = Enum.Parse<TipoIdentificacion>(dto.TipoIdentificacion.ToUpper()),
            Identificacion = dto.Identificacion,
            Email = dto.Email?.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Direccion = dto.Direccion?.Trim(),
            CreatedBy = userId
        };

        await _repo.AddAsync(cliente);
        return cliente.Id;
    }

    public async Task ActualizarAsync(int id, ActualizarClienteDto dto, int userId)
    {
        var cliente = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Cliente no encontrado");

        cliente.Nombre = dto.Nombre.Trim();
        cliente.Email = dto.Email?.Trim();
        cliente.Telefono = dto.Telefono?.Trim();
        cliente.Direccion = dto.Direccion?.Trim();

        await _repo.UpdateAsync(cliente);
    }

    public async Task DesactivarAsync(int id)
    {
        await _repo.DesactivarAsync(id);
    }

    private static ClienteDto ToDto(Cliente c) => new(
        c.Id,
        c.Nombre,
        c.TipoIdentificacion.ToString(),
        c.Identificacion,
        c.Email,
        c.Telefono,
        c.Direccion,
        c.Activo
    );

    private static void ValidarIdentificacion(string tipoStr, string numero)
    {
        if (!Enum.TryParse<TipoIdentificacion>(tipoStr.ToUpper(), out var tipo))
            throw new ArgumentException("Tipo de identificación inválido");

        if (string.IsNullOrWhiteSpace(numero) || !numero.All(char.IsDigit))
            throw new ArgumentException("La identificación debe contener solo números");

        switch (tipo)
        {
            case TipoIdentificacion.CEDULA when numero.Length != 10:
            case TipoIdentificacion.RUC when numero.Length != 13:
            case TipoIdentificacion.PASAPORTE when numero.Length < 5:
                throw new ArgumentException($"Longitud inválida para {tipo}");
        }
    }
}