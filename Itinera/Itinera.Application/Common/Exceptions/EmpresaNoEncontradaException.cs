namespace Itinera.Application.Common.Exceptions;

public sealed class EmpresaNoEncontradaException : NotFoundException
{
    public EmpresaNoEncontradaException(object id)
        : base("Empresa", id)
    {
    }
}
