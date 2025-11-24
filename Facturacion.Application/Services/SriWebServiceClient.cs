using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Http;
using System.Xml;
using Facturacion.Application.Config;
using Microsoft.Extensions.Options;

namespace Facturacion.Application.Services;

public class SriWebServiceClient
{
    private readonly SriConfig _config;

    public SriWebServiceClient(IOptions<SriConfig> config)
    {
        _config = config.Value;
    }

    public async Task<(bool exitoso, string mensaje, string? numeroAutorizacion, DateTime? fechaAutorizacion)> EnviarXmlParaRecepcion(string xmlFirmado)
    {
        try
        {
            // 1. Crear el cliente del web service de recepción
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.MaxReceivedMessageSize = int.MaxValue; // Para manejar XML grandes

            var endpoint = new EndpointAddress(_config.WsdlRecepcion);
            var client = new RecepcionComprobantes.RecepcionComprobantesServiceClient(binding, endpoint);

            // 2. Convertir el string XML a un objeto XMLDocument
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFirmado);

            // 3. Llamar al método del web service
            var respuesta = await client.validarComprobanteAsync(xmlDoc);

            // 4. Interpretar la respuesta
            if (respuesta.estado == "RECIBIDA")
            {
                // Si fue recibida, ahora debes llamar a autorización
                // (esto es otro paso, no lo hago aquí)
                return (true, "Comprobante recibido por el SRI", null, null);
            }
            else if (respuesta.estado == "DEVUELTA")
            {
                var errores = string.Join(", ", respuesta.comprobantes.comprobante.Select(c => c.mensajes.mensaje.FirstOrDefault()?.mensaje ?? "Error desconocido"));
                return (false, $"Comprobante rechazado: {errores}", null, null);
            }
            else
            {
                return (false, $"Estado desconocido: {respuesta.estado}", null, null);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Error de comunicación con el SRI: {ex.Message}", null, null);
        }
    }

    // Método para autorización (esto se llama después de recepción exitosa)
    public async Task<(bool exitoso, string mensaje, string? numeroAutorizacion, DateTime? fechaAutorizacion)> ConsultarAutorizacion(string claveAcceso)
    {
        try
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.MaxReceivedMessageSize = int.MaxValue;

            var endpoint = new EndpointAddress(_config.WsdlAutorizacion);
            var client = new AutorizacionComprobantes.AutorizacionComprobantesServiceClient(binding, endpoint);

            var respuesta = await client.autorizarComprobanteAsync(claveAcceso);

            if (respuesta.autorizaciones.autorizacion.FirstOrDefault()?.estado == "AUTORIZADO")
            {
                var autorizacion = respuesta.autorizaciones.autorizacion.First();
                return (true, "Comprobante autorizado", autorizacion.numeroAutorizacion, autorizacion.fechaAutorizacion);
            }
            else
            {
                var mensajes = respuesta.autorizaciones.autorizacion.FirstOrDefault()?.mensajes?.mensaje?.Select(m => m.mensaje)?.FirstOrDefault();
                return (false, $"Comprobante no autorizado: {mensajes}", null, null);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Error de comunicación con el SRI (autorización): {ex.Message}", null, null);
        }
    }
}