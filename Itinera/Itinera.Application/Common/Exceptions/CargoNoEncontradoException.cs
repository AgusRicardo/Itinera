namespace Itinera.Application.Common.Exceptions;

public sealed class CargoNoEncontradoException : NotFoundException
{
    public CargoNoEncontradoException(object id)
        : base("Cargo", id)
    {
    }
}
