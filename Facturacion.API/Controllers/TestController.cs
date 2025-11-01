using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult GetPublic() => Ok("Acceso público");

        [HttpGet("user")]
        [Authorize]
        public IActionResult GetUser() => Ok($"Hola {User.Identity?.Name}, eres usuario autenticado");

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdmin() => Ok("Solo admins!");
    }
}