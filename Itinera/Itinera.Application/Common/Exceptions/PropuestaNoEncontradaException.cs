namespace Itinera.Application.Common.Exceptions;

public sealed class PropuestaNoEncontradaException : NotFoundException
{
    public PropuestaNoEncontradaException(object id)
        : base("Propuesta", id)
    {
    }
}
