using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Aplicacion.Usuarios.Commands 
{
    // Devuelve el ID creado
    public class CrearUsuarioCommand : IRequest<int>
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$",
            ErrorMessage = "El nombre solo puede contener letras.")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}





