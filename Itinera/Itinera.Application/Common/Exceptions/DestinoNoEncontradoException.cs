namespace Itinera.Application.Common.Exceptions;

public sealed class DestinoNoEncontradoException : NotFoundException
{
    public DestinoNoEncontradoException(object id)
        : base("Destino", id)
    {
    }
}
