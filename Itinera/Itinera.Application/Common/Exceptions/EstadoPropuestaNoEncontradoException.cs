namespace Itinera.Application.Common.Exceptions;

public sealed class EstadoPropuestaNoEncontradoException : NotFoundException
{
    public EstadoPropuestaNoEncontradoException(object id)
        : base("Estado de propuesta", id)
    {
    }
}
