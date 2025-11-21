using Facturacion.Application.Interfaces;
using Facturacion.Domain.Entities;
using Facturacion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Facturacion.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // 1. Corregimos el nombre del parámetro de 'username' a 'email'
        public async Task<string?> LoginAsync(string email, string password)
        {
            // 2. CORRECCIÓN CLAVE: Buscamos al usuario por el campo Email
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            // Verificación de existencia y de hash
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Usuario user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            // Verifica si ya existe un usuario con el mismo email (caso insensible a mayúsculas/minúsculas)
            return await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<Usuario?> RegisterAsync(Usuario user, string password)
        {
            // 1. Verificación de existencia (Opcional, pero recomendado en un servicio de registro)
            if (await UserExistsAsync(user.Email))
            {
                return null; // O podrías lanzar una excepción específica
            }

            // 2. HASHEADO SEGURO: Generamos el hash de la contraseña usando BCrypt
            // El '10' es el costo (cost factor) recomendado para BCrypt.
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, 10);

            // 3. Asignamos el hash al objeto Usuario
            user.PasswordHash = passwordHash;

            // 4. Agregamos el nuevo usuario al contexto y guardamos los cambios
            await _context.Usuarios.AddAsync(user);
            await _context.SaveChangesAsync();

            // Devolvemos el usuario registrado (sin el hash)
            return user;
        }
    }
}