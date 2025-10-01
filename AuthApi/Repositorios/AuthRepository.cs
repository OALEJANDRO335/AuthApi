using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApi.Interfaces;
using AuthApi.Servicios;
using AuthApi.Entidades;
using AuthApi.DTOs.UsuarioDTOs;
using System;
using Microsoft.Extensions.Configuration; // Necesario para IConfiguration
using BCrypt.Net; // Asegúrate de que este using es correcto para tu librería de hashing

namespace AuthApi.Repositorios
{
    // Implementa el servicio de autenticación
    public class AuthRepository : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IConfiguration _config;

        public AuthRepository(IUsuarioRepository usuarioRepo, IConfiguration config)
        {
            _usuarioRepo = usuarioRepo;
            _config = config;
        }

        // -----------------------------------------------------------------
        // MÉTODO DE REGISTRO
        // -----------------------------------------------------------------
        public async Task<UsuarioRespuestaDTO> RegistrarAsync(UsuarioRegistroDTO dto)
        {

            // 1. Creación del objeto Usuario (Entidad)
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                // Hasheo de la contraseña con BCrypt
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                // 🔑 CORRECCIÓN APLICADA: Asignación del RolId por defecto
                // Esto resuelve el error de Foreign Key (DbUpdateException)
                // Asume que el ID 2 corresponde al Rol 'Usuario' estándar.
                RolId = 5
            };


            // 2. Guardar el nuevo usuario en la base de datos
            await _usuarioRepo.AddAsync(usuario);

            // 3. Obtener la entidad completa de nuevo (con el ID y la navegación del Rol)
            // Esta llamada es necesaria porque AddAsync puede no cargar la propiedad Rol
            // Si tu repositorio no carga la propiedad Rol, el GenerarToken fallará.
            usuario = await _usuarioRepo.GetByEmailAsync(usuario.Email);

            // 4. Generar el Token JWT
            string token = GenerarToken(usuario);

            // 5. Devolver la respuesta al cliente
            return new UsuarioRespuestaDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                // El '?' permite usar el valor por defecto "Usuario" si la propiedad Rol no se cargó correctamente
                Rol = usuario.Rol?.Nombre ?? "Usuario",
                Token = token
            };
        }

        // -----------------------------------------------------------------
        // MÉTODO DE INICIO DE SESIÓN
        // -----------------------------------------------------------------
        public async Task<UsuarioRespuestaDTO?> LoginAsync(UsuarioLoginDTO dto)
        {
            var usuario = await _usuarioRepo.GetByEmailAsync(dto.Email);

            // 1. Verificar si el usuario existe
            if (usuario == null) return null;

            // 2. Verificar la contraseña (comparar hash)
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return null; // Contraseña inválida

            // 3. Devolver la respuesta con el Token
            return new UsuarioRespuestaDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                // Asume que el Rol está cargado por el GetByEmailAsync
                Rol = usuario.Rol.Nombre,
                Token = GenerarToken(usuario)
            };
        }

        // -----------------------------------------------------------------
        // MÉTODO PRIVADO: GENERAR TOKEN JWT
        // -----------------------------------------------------------------
        private string GenerarToken(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            // Esta verificación asegura que la propiedad de navegación Rol NO es nula antes de intentar acceder a .Nombre
            if (usuario.Rol == null) throw new InvalidOperationException("El usuario no tiene rol asignado (Propiedad Rol es null).");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims del Token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email ?? ""),
                new Claim("rol", usuario.Rol.Nombre)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}