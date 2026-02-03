using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Dominio.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        // Validaciones específicas (puedes usar esto más adelante en reglas de negocio)
        public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
        public Usuario(string nombre, string email, string passwordHash)
        {
            if (!EsEmailValido(email))
                throw new FormatException("The email address is not valid.");

            Nombre = nombre;
            Email = email;
            PasswordHash = passwordHash;
            UsuarioRoles = new List<UsuarioRol>();
        }
        public Usuario() 
        {
            UsuarioRoles = new List<UsuarioRol>();
        }
        // Método de validación de email
        private bool EsEmailValido(string email)
        {
            // Validación básica del formato de email usando Regex
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}
