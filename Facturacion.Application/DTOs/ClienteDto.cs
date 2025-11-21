using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Application.DTOs;

public record ClienteDto(
    int Id,
    string Nombre,
    string TipoIdentificacion,
    string Identificacion,
    string? Email,
    string? Telefono,
    string? Direccion,
    bool Activo);

public record CrearClienteDto(
    string Nombre,
    string TipoIdentificacion,
    string Identificacion,
    string? Direccion,
    string? Telefono,
    string? Email);

public record ActualizarClienteDto(
    string Nombre,
    string? Direccion,
    string? Telefono,
    string? Email);
