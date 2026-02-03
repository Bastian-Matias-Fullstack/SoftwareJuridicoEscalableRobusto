using Aplicacion.DTO;
using FluentValidation;

namespace Aplicacion.Validaciones
{
    public  class CrearCasoRequestValidator : AbstractValidator<CrearCasoRequest>
    {
        public CrearCasoRequestValidator() 
        {
            RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título es obligatorio.");

            RuleFor(x => x.Descripcion)
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres.");
          
            RuleFor(x => x.ClienteId)
         .GreaterThan(0).WithMessage("Debe seleccionar un cliente válido.");

            RuleFor(x => x.TipoCaso)
                          .IsInEnum()
                          .WithMessage("Tipo de caso no válido.");
        }
    }
}
