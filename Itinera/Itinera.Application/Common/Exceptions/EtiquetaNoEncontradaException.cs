namespace Itinera.Application.Common.Exceptions;

public sealed class EtiquetaNoEncontradaException(object id)
    : NotFoundException("Etiqueta", id);
