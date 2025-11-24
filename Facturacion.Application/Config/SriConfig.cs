using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturacion.Application.Config;

public class SriConfig
{
    public const string SectionName = "Sri";
    public string Ambiente { get; set; } = "Pruebas"; // "Pruebas" o "Produccion"
    public string WsdlRecepcion { get; set; } = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantes?wsdl";
    public string WsdlAutorizacion { get; set; } = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantes?wsdl";
}