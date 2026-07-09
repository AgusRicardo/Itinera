namespace Itinera.Application.Common.Exceptions;

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string entidad, object id)
        : base($"{entidad} con Id '{id}' no fue encontrado.")
    {
    }
}