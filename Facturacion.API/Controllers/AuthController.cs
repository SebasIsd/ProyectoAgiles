using Facturacion.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.Email, dto.Password); 
            
            if (token == null)
                return Unauthorized("Credenciales incorrectas");

            return Ok(new { token });
        }
    }

    public class LoginDto
    {
        
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}