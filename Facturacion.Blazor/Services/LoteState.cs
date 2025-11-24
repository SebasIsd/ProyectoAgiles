// Facturacion.Blazor/Services/LoteState.cs
using Facturacion.Blazor.Models;

namespace Facturacion.Blazor.Services
{
    public class LoteState
    {
        private readonly ILoteApiService _loteApiService;
        private List<LoteItem> _lotes = new();

        public LoteState(ILoteApiService loteApiService)
        {
            _loteApiService = loteApiService;
        }

        public void Add(LoteItem lote)
        {
            _lotes.Add(lote);
        }

        public IReadOnlyList<LoteItem> GetAll()
        {
            return _lotes.AsReadOnly();
        }

        public async Task CargarLotesAsync(string? searchTerm = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    _lotes = await _loteApiService.SearchAsync(searchTerm);
                }
                else
                {
                    _lotes = await _loteApiService.GetAllAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando lotes: {ex.Message}");
                _lotes = new List<LoteItem>();
            }
        }

        public async Task<LoteItem?> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _loteApiService.GetByIdAsync(id);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> GuardarLoteAsync(LoteItem lote)
        {
            try
            {
                bool resultado;
                if (lote.Id == 0)  // ⭐⭐ CAMBIÉ LoteId por Id ⭐⭐
                {
                    resultado = await _loteApiService.CreateAsync(lote);
                }
                else
                {
                    resultado = await _loteApiService.UpdateAsync(lote);
                }

                if (resultado)
                {
                    await CargarLotesAsync(); // Recargar la lista
                }

                return resultado;
            }
            catch
            {
                return false;
            }
        }
    }
}