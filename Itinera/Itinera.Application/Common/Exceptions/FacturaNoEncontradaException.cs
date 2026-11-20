namespace Itinera.Application.Common.Exceptions;

public sealed class FacturaNoEncontradaException(object id)
    : NotFoundException("Factura", id);
