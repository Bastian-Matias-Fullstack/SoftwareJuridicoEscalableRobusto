using Dominio.Entidades;

namespace Aplicacion.DTOs
{
    public class CasoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public EstadoCaso Estado { get; set; } = EstadoCaso.Pendiente;
        public DateTimeOffset FechaCreacion { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        public TipoCaso TipoCaso { get; set; }  // ¡Agrega este campo!
        public string Descripcion { get; set; } = string.Empty; // ✅ Este campo es el que falta
        public string? MotivoCierre { get; set; }
    }
}
