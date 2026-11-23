namespace Itinera.Application.Common.Exceptions;

public sealed class ServicioIANoDisponibleException(string mensaje, Exception? innerException = null)
    : Exception(mensaje, innerException);
