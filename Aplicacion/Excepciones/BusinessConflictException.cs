namespace Aplicacion.Excepciones;
public class BusinessConflictException : DomainException
{
    public BusinessConflictException(string mensaje) : base(mensaje) { }
}
