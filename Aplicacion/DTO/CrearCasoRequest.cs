using System.ComponentModel.DataAnnotations;

namespace Aplicacion.DTO
{
    public class CrearCasoRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 5)]
        public string Titulo { get; set; } = string.Empty;
        [StringLength(5000)]
        public string Descripcion {  get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int ClienteId { get; set; }
        [Required]
        public TipoCaso TipoCaso { get; set; }
    }
}
