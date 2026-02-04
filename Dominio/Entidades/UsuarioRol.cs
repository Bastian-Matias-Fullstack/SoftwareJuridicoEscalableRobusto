using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class UsuarioRol
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;
    }
}
