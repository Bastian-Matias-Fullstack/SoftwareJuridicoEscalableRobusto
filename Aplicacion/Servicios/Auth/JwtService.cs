using Dominio.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aplicacion.Servicios.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerarToken(string email, int usuarioId, List<string> roles)
        {
            var jwt = _config.GetSection("Jwt");

            // 1) Leer valores con validación (evita nulls + elimina CS8604)
            var keyStr = jwt["Key"];
            if (string.IsNullOrWhiteSpace(keyStr))
                throw new InvalidOperationException("JWT Key no configurada: Jwt:Key");

            var issuer = jwt["Issuer"];
            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT Issuer no configurado: Jwt:Issuer");

            var audience = jwt["Audience"];
            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT Audience no configurado: Jwt:Audience");

            // 2) Expires seguro (evita double.Parse null/invalid)
            var expStr = jwt["ExpiresInMinutes"];
            if (!double.TryParse(expStr, out var expMinutes))
                expMinutes = 60; // default razonable para demo

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims base
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Name, email),
            };

            // Claims de roles
            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}