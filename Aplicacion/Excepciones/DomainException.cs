namespace Aplicacion.Excepciones;
public abstract class DomainException : Exception
{
    protected DomainException(string mensaje) : base(mensaje)
    {
    }
}
