namespace Itinera.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UsuarioId { get; }
    bool TienePermiso(string codigoPermiso);
}
