namespace Itinera.Application.Common.Exceptions;

public sealed class ConceptoFacturaNoEncontradoException(object id)
    : NotFoundException("Concepto", id);
