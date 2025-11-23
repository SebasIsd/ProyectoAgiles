using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Facturacion.Domain.Entities{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoIdentificacion
    {
        [Display(Name = "Cédula")]
        CEDULA = 1,

        [Display(Name = "RUC")]
        RUC = 2,

        [Display(Name = "Pasaporte")]
        PASAPORTE = 3
    }
}