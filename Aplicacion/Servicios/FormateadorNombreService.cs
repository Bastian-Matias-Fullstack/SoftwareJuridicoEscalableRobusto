
namespace Aplicacion.Servicios
{
    public class FormateadorNombreService
    {
        public string Formatear(string nombre)
        {
            return nombre.Trim().ToUpper();
        }
    }
}
