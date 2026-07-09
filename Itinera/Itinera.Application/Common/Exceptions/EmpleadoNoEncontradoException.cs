namespace Itinera.Application.Common.Exceptions;

public sealed class EmpleadoNoEncontradoException : NotFoundException
{
    public EmpleadoNoEncontradoException(object id)
        : base("Empleado", id)
    {
    }
}
