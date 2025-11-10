using System.ComponentModel.DataAnnotations;

namespace Facturacion.Blazor.Models // Asegúrate de que el namespace sea correcto
{
    public class LoginVm
    {
        [Required, EmailAddress] public string Correo { get; set; } = "";
        [Required, MinLength(4)] public string Password { get; set; } = "";
    }
}