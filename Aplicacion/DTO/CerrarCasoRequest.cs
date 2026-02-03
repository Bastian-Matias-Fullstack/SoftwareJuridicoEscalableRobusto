using System.ComponentModel.DataAnnotations;

namespace Aplicacion.DTO
{
    public class CerrarCasoRequest
    {
        [StringLength(1000)]
        public string? MotivoCierre { get; set; }
    }
}
