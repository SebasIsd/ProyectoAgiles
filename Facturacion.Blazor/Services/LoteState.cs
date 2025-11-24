using System;
using System.Collections.Generic;
using System.Linq;

namespace Facturacion.Blazor.Services
{
    public class LoteItem
    {
        public int LoteId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoCodigo { get; set; } = string.Empty;
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoCategoria { get; set; } = string.Empty;
        public DateTime FechaCompra { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PVP { get; set; }
        public int CantidadInicial { get; set; }
        public int CantidadDisponible { get; set; }

        // Importación
        public bool EsImportado { get; set; }
        public bool EsImportacion4x4 { get; set; }

        // Estado del lote (Activo, Agotado, Vencido, etc.)
        public string Estado { get; set; } = "Activo";
    }

    public class LoteState
    {
        private readonly List<LoteItem> _lotes = new();

        public LoteState()
        {
            // Ejemplo inicial
            _lotes.Add(new LoteItem
            {
                LoteId = 1,
                ProductoId = 10,
                ProductoCodigo = "PROD-001",
                ProductoNombre = "Mouse óptico",
                ProductoCategoria = "Periféricos",
                FechaCompra = DateTime.Today.AddDays(-10),
                FechaExpiracion = null,
                PrecioCosto = 8.50m,
                PVP = 12.99m,
                CantidadInicial = 50,
                CantidadDisponible = 40,
                EsImportado = true,
                EsImportacion4x4 = false
            });

            // Recalcular el estado de los lotes iniciales
            foreach (var lote in _lotes)
            {
                RecalcularEstado(lote);
            }
        }

        public IReadOnlyList<LoteItem> GetAll()
        {
            // Por si cambia la fecha actual, recalculamos estados al leer
            foreach (var lote in _lotes)
            {
                RecalcularEstado(lote);
            }

            return _lotes;
        }

        public LoteItem? GetById(int loteId) =>
            _lotes.FirstOrDefault(l => l.LoteId == loteId);

        public LoteItem Add(LoteItem lote)
        {
            lote.LoteId = _lotes.Any() ? _lotes.Max(l => l.LoteId) + 1 : 1;

            RecalcularEstado(lote);
            _lotes.Add(lote);

            return lote;
        }

        public void Update(LoteItem lote)
        {
            var existente = GetById(lote.LoteId);
            if (existente is null) return;

            existente.FechaExpiracion = lote.FechaExpiracion;
            existente.PrecioCosto = lote.PrecioCosto;
            existente.PVP = lote.PVP;
            existente.CantidadDisponible = lote.CantidadDisponible;
            existente.EsImportado = lote.EsImportado;
            existente.EsImportacion4x4 = lote.EsImportacion4x4;

            RecalcularEstado(existente);
        }

        /// <summary>
        /// Calcula el estado del lote según cantidad disponible y fecha de expiración.
        /// </summary>
        private void RecalcularEstado(LoteItem lote)
        {
            // 1) Si no hay stock → Agotado (tiene prioridad)
            if (lote.CantidadDisponible <= 0)
            {
                lote.Estado = "Agotado";
                return;
            }

            // 2) Si tiene fecha de expiración y ya pasó → Vencido
            if (lote.FechaExpiracion.HasValue &&
                lote.FechaExpiracion.Value.Date < DateTime.Today)
            {
                lote.Estado = "Vencido";
                return;
            }

            // 3) En cualquier otro caso → Activo
            lote.Estado = "Activo";
        }
    }
}