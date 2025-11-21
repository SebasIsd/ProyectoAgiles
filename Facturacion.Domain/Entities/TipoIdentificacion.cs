using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Domain.Entities;

public enum TipoIdentificacion
{
    [Display(Name = "Cédula")]
    CEDULA = 1,

    [Display(Name = "RUC")]
    RUC = 2,

    [Display(Name = "Pasaporte")]
    PASAPORTE = 3
}