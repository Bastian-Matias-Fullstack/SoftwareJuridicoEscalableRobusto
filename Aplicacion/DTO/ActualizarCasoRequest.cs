using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs
{
    //solo lo que el cliente pueda modificar
    public class ActualizarCasoRequest
    {
        [StringLength(150, MinimumLength = 5)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(5000)]
        public string Descripcion { get; set; } = string.Empty;
        public TipoCaso TipoCaso { get; set; }

        [Range(1, int.MaxValue)]
        public int ClienteId { get; set; }   // 🔥 CLAVE
        //public EstadoCaso Estado { get; set; } // 👈 Agregado para mejora 
    }
}
