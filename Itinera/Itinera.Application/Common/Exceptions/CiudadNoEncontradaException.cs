namespace Itinera.Application.Common.Exceptions;

public sealed class CiudadNoEncontradaException : NotFoundException
{
    public CiudadNoEncontradaException(object id)
        : base("Ciudad", id)
    {
    }
}
