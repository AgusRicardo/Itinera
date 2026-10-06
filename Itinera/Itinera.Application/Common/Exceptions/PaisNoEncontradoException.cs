namespace Itinera.Application.Common.Exceptions;

public sealed class PaisNoEncontradoException : NotFoundException
{
    public PaisNoEncontradoException(object id)
        : base("Pais", id)
    {
    }
}
