using System.Xml;
using System.Xml.Linq;
using Facturacion.Application.DTOs;
using Facturacion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Facturacion.Application.Services;

public class SriXmlGeneratorService
{
    private readonly EmpresaDto _empresa;

    public SriXmlGeneratorService(EmpresaDto empresa)
    {
        _empresa = empresa;
    }

    public string GenerarXmlFactura(Factura factura, List<FacturaDetalleDto> detalles)
    {
        var claveAcceso = GenerarClaveAcceso(factura);

        var facturaXml = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement("factura",
                new XAttribute("id", "comprobante"),
                new XAttribute("version", "1.1.0"),
                new XElement("infoTributaria",
                    new XElement("ambiente", factura.Ambiente.ToString()), // 1=pruebas, 2=producción
                    new XElement("tipoEmision", "1"), // 1=normal
                    new XElement("razonSocial", _empresa.RazonSocial),
                    new XElement("nombreComercial", _empresa.NombreComercial),
                    new XElement("ruc", _empresa.Ruc),
                    new XElement("claveAcceso", claveAcceso),
                    new XElement("codDoc", "01"), // 01=factura
                    new XElement("estab", factura.Numero.Substring(0, 3)), // 001
                    new XElement("ptoEmi", factura.Numero.Substring(4, 3)), // 001
                    new XElement("secuencial", factura.Numero.Substring(8, 9)), // 000000001
                    new XElement("dirMatriz", _empresa.DireccionMatriz)
                ),
                new XElement("infoFactura",
                    new XElement("fechaEmision", factura.FechaEmision.ToString("dd/MM/yyyy")),
                    new XElement("dirEstablecimiento", _empresa.DireccionMatriz),
                    // Opcional: Contribuyente especial
                    // if (!string.IsNullOrEmpty(_empresa.ContribuyenteEspecial))
                    //     new XElement("contribuyenteEspecial", _empresa.ContribuyenteEspecial),
                    new XElement("obligadoContabilidad", _empresa.ObligadoContabilidad ? "SI" : "NO"),
                    new XElement("tipoIdentificacionComprador", ObtenerCodigoTipoIdentificacion(factura.Cliente.TipoIdentificacion.ToString())),
                    new XElement("razonSocialComprador", factura.Cliente.Nombre),
                    new XElement("identificacionComprador", factura.Cliente.Identificacion),
                    new XElement("totalSinImpuestos", factura.Subtotal.ToString("F2")),
                    new XElement("totalDescuento", "0.00"), // Ajustar si aplica
                    new XElement("totalConImpuestos",
                        new XElement("totalImpuesto",
                            new XElement("codigo", "2"), // 2=IVA
                            new XElement("codigoPorcentaje", "2"), // 2=12%
                            new XElement("baseImponible", factura.Subtotal.ToString("F2")),
                            new XElement("valor", factura.Iva.ToString("F2"))
                        )
                    ),
                    new XElement("importeTotal", factura.Total.ToString("F2")),
                    new XElement("moneda", "DOLAR")
                ),
                new XElement("detalles",
                    detalles.Select(d => new XElement("detalle",
                        new XElement("codigoPrincipal", d.ProductoCodigo ?? ""),
                        new XElement("descripcion", d.ProductoNombre),
                        new XElement("cantidad", d.Cantidad.ToString("F2")),
                        new XElement("precioUnitario", d.PrecioUnitario.ToString("F2")),
                        new XElement("descuento", "0.00"), // Puedes ajustar si el detalle tiene descuento
                        new XElement("precioTotalSinImpuesto", d.SubtotalLinea.ToString("F2")),
                        new XElement("impuestos",
                            new XElement("impuesto",
                                new XElement("codigo", "2"), // 2=IVA
                                new XElement("codigoPorcentaje", "2"), // 2=12%
                                new XElement("tarifa", "12.00"),
                                new XElement("baseImponible", d.SubtotalLinea.ToString("F2")),
                                new XElement("valor", d.ValorIva.ToString("F2"))
                            )
                        )
                    ))
                ),
                new XElement("infoAdicional",
                    !string.IsNullOrEmpty(factura.Cliente.Direccion) ?
                        new XElement("campoAdicional", new XAttribute("nombre", "direccion"), factura.Cliente.Direccion) : null,
                    !string.IsNullOrEmpty(factura.Cliente.Email) ?
                        new XElement("campoAdicional", new XAttribute("nombre", "email"), factura.Cliente.Email) : null
                )
            )
        );

        return facturaXml.ToString();
    }

    private string GenerarClaveAcceso(Factura factura)
    {
        var fecha = factura.FechaEmision.ToString("ddMMyyyy");
        var tipoComprobante = "01"; // Factura
        var ruc = _empresa.Ruc;
        var ambiente = factura.Ambiente.ToString(); // 1 o 2
        var serie = factura.Numero.Substring(0, 6); // 001001
        var secuencial = factura.Numero.Substring(8, 9); // 000000001
        var codigoNumerico = "12345678"; // Puedes generar uno aleatorio o usar un ID
        var tipoEmision = "1"; // Normal

        var claveSinModulo = $"{fecha}{tipoComprobante}{ruc}{ambiente}{serie}{secuencial}{codigoNumerico}{tipoEmision}";

        // Algoritmo de módulo 11 para dígito verificador
        var digitoVerificador = CalcularModulo11(claveSinModulo);

        return claveSinModulo + digitoVerificador;
    }

    private string CalcularModulo11(string valor)
    {
        int suma = 0;
        int[] coeficientes = { 2, 7, 6, 5, 4, 3, 2 };
        int indiceCoeficiente = 0;

        for (int i = valor.Length - 1; i >= 0; i--)
        {
            int digito = int.Parse(valor[i].ToString());
            suma += digito * coeficientes[indiceCoeficiente];
            indiceCoeficiente = (indiceCoeficiente + 1) % coeficientes.Length;
        }

        int residuo = suma % 11;
        int digitoVerificador = 11 - residuo;
        if (digitoVerificador >= 10) digitoVerificador = digitoVerificador - 10;

        return digitoVerificador.ToString();
    }

    private string ObtenerCodigoTipoIdentificacion(string tipoIdentificacion)
    {
        // Ajusta según tu catálogo TipoIdentificacion
        return tipoIdentificacion switch
        {
            "CEDULA" => "05",
            "RUC" => "04",
            "PASAPORTE" => "06",
            _ => "07" // Otros
        };
    }
}