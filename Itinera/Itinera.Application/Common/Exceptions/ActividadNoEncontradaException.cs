namespace Itinera.Application.Common.Exceptions;

public sealed class ActividadNoEncontradaException : NotFoundException
{
    public ActividadNoEncontradaException(object id)
        : base("Actividad", id)
    {
    }
}
