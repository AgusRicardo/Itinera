namespace Itinera.Application.Common.Exceptions;

public sealed class ClienteNoEncontradoException : NotFoundException
{
    public ClienteNoEncontradoException(object id)
        : base("Cliente", id)
    {
    }
}
