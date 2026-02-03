using MediatR;
namespace Aplicacion.Usuarios.Commands
{
    public class ActualizarUsuarioCommand : IRequest<Unit> 
    {
        public int Id { get; set; } 
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } 

        public ActualizarUsuarioCommand(
            int id,
            string nombre,
            string email,
            string? password)
        {
            Id = id;
            Nombre = nombre;
            Email = email;
            Password = password;
        }
    }
}

  

    

