namespace Aplicacion.Excepciones;
public class NotFoundException : DomainException
{
    public NotFoundException(string mensaje) : base(mensaje) { }
}
