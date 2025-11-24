using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Application.DTOs;

public class EmpresaDto
{
    public int Id { get; set; }
    public string Ruc { get; set; } = null!;
    public string RazonSocial { get; set; } = null!;
    public string? NombreComercial { get; set; }
    public string? DireccionMatriz { get; set; }
    public string? ContribuyenteEspecial { get; set; }
    public bool ObligadoContabilidad { get; set; }
    public string? LogoPath { get; set; }
    public bool Activo { get; set; }
}

public class CrearEmpresaDto
{
    public string Ruc { get; set; } = null!;
    public string RazonSocial { get; set; } = null!;
    public string? NombreComercial { get; set; }
    public string? DireccionMatriz { get; set; }
    public string? ContribuyenteEspecial { get; set; }
    public bool ObligadoContabilidad { get; set; } = true;
    public string? LogoPath { get; set; }
}

public class ActualizarEmpresaDto
{
    public string RazonSocial { get; set; } = null!;
    public string? NombreComercial { get; set; }
    public string? DireccionMatriz { get; set; }
    public string? ContribuyenteEspecial { get; set; }
    public bool ObligadoContabilidad { get; set; }
    public string? LogoPath { get; set; }
    public bool Activo { get; set; } = true;
}