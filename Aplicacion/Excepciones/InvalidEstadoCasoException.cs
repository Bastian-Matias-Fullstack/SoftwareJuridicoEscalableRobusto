namespace Aplicacion.Excepciones;
public class InvalidEstadoCasoException : DomainException
{
    public InvalidEstadoCasoException(string mensaje) : base(mensaje)
    {
    }
}
