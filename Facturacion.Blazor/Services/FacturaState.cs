using Facturacion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facturacion.Blazor.Services
{
    public class FacturaItem
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Today;
        public string Cliente { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }

    public class FacturaState
    {
        private readonly List<FacturaItem> _facturas = new();
        private List<FacturaDto> _facturasDto = new();
        private FacturaApiService? _facturaApiService;

        public void SetApiService(FacturaApiService facturaApiService)
        {
            _facturaApiService = facturaApiService;
        }
        public IReadOnlyList<FacturaDto> GetAll() => _facturasDto.AsReadOnly();
        public async Task LoadFacturasAsync()
        {
            if (_facturaApiService == null)
                throw new InvalidOperationException("FacturaApiService no ha sido inyectado en FacturaState.");

            var facturas = await _facturaApiService.GetAllAsync();
            _facturasDto = facturas ?? new List<FacturaDto>();
        }



        public FacturaState()
        {
            // Factura de ejemplo inicial (la que ya veías en la tabla)
            _facturas.Add(new FacturaItem
            {
                Id = 1,
                Numero = "001-001-000000010",
                Fecha = new DateTime(2025, 11, 22),
                Cliente = "Carlos Pérez",
                Subtotal = 100m,
                Iva = 12m,
                Total = 112m,
                Estado = "Pagada"
            });
        }

        //public IReadOnlyList<FacturaItem> GetAll() => _facturas;

        public FacturaItem? GetById(int id) =>
            _facturas.FirstOrDefault(f => f.Id == id);

        public FacturaItem Add(FacturaItem factura)
        {
            factura.Id = _facturas.Any() ? _facturas.Max(f => f.Id) + 1 : 1;
            _facturas.Add(factura);
            return factura;
        }

        public void Update(FacturaItem factura)
        {
            var existente = GetById(factura.Id);
            if (existente is null) return;

            existente.Numero = factura.Numero;
            existente.Fecha = factura.Fecha;
            existente.Cliente = factura.Cliente;
            existente.Subtotal = factura.Subtotal;
            existente.Iva = factura.Iva;
            existente.Total = factura.Total;
            existente.Estado = factura.Estado;
        }
    }
}