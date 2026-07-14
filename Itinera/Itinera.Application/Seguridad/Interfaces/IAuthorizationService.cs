namespace Itinera.Application.Seguridad.Interfaces;

public interface IAuthorizationService
{
    Task<bool> VerificarPermisoAsync(Guid usuarioId, string codigoPermiso);
    Task<List<string>> ObtenerPermisosAsync(Guid usuarioId);
}
