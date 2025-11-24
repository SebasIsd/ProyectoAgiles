// Services/AlertService.cs
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;

namespace Facturacion.Blazor.Services;

public class AlertService
{
    private readonly SweetAlertService _swal;

    public AlertService(SweetAlertService swal) => _swal = swal;

    public async Task Success(string title = "Éxito", string text = "Operación completada correctamente")
        => await _swal.FireAsync(title, text, SweetAlertIcon.Success);

    public async Task Error(string text, string title = "Error")
        => await _swal.FireAsync(title, text, SweetAlertIcon.Error);

    public async Task<bool> Confirm(string title = "¿Estás seguro?", string text = "Esta acción no se puede deshacer")
    {
        var result = await _swal.FireAsync(new SweetAlertOptions
        {
            Title = title,
            Text = text,
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
            ConfirmButtonText = "Sí, continuar",
            CancelButtonText = "Cancelar"
        });
        return result.IsConfirmed;
    }

    public async Task Info(string text, string title = "Información")
        => await _swal.FireAsync(title, text, SweetAlertIcon.Info);

    public enum AlertType
    {
        Success,
        Error,
        Warning,
        Info
    }
        public string Mensaje { get; private set; } = string.Empty;
        public AlertType Tipo { get; private set; } = AlertType.Info;
        public bool HasAlert => !string.IsNullOrEmpty(Mensaje);

        public void Mostrar(string mensaje, AlertType tipo = AlertType.Info)
        {
            Mensaje = mensaje;
            Tipo = tipo;
        }

        public void Limpiar()
        {
            Mensaje = string.Empty;
            Tipo = AlertType.Info;
        }
}