namespace Facturacion.Blazor.Services
{
    public interface IAuthService
    {
        Task Login(string username, string password);
        Task Logout();
    }
}