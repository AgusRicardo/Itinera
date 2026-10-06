namespace Itinera.Application.Common.Exceptions;

public sealed class EstadoFacturaNoEncontradaException : NotFoundException
{
    public EstadoFacturaNoEncontradaException(object id)
        : base("Estado de factura", id)
    {
    }
}
