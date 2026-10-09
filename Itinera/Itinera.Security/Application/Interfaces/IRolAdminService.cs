using Itinera.Security.Application.Dtos;

namespace Itinera.Security.Application.Interfaces;

public interface IRolAdminService
{
    Task<List<RolAdminResponse>> ListarAsync();
    Task<List<PermisoAdminResponse>> ListarPermisosAsync();
    Task<RolAdminResponse> CrearAsync(CrearRolRequest request);
    Task AsignarPermisoAsync(int rolId, int permisoId);
    Task QuitarPermisoAsync(int rolId, int permisoId);
}
