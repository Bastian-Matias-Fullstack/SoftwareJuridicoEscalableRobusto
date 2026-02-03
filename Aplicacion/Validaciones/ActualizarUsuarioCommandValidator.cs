using Aplicacion.Usuarios.Commands;
using FluentValidation;

namespace Aplicacion.Usuarios.Validators
{
    public class ActualizarUsuarioCommandValidator
        : AbstractValidator<ActualizarUsuarioCommand>
    {
        public ActualizarUsuarioCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El Id del usuario es inválido.");

            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MinimumLength(3)
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$")
                .WithMessage("El nombre solo puede contener letras.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Email inválido.");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.Password))
                .WithMessage("La contraseña debe tener al menos 6 caracteres.");
        }
    }
}
