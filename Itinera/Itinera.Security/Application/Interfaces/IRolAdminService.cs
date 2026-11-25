using System.Security.Claims;
using Itinera.Security.Application.Dtos;

namespace Itinera.Security.Application.Interfaces;

public interface IRolAdminService
{
    Task<List<RolAdminResponse>> ObtenerTodosAsync();
    Task<List<PermisoAdminResponse>> ObtenerPermisosAsync(ClaimsPrincipal usuario);
    Task<RolAdminResponse> CrearAsync(CrearRolRequest request, ClaimsPrincipal usuario);
    Task AsignarPermisoAsync(int rolId, int permisoId, ClaimsPrincipal usuario);
    Task QuitarPermisoAsync(int rolId, int permisoId, ClaimsPrincipal usuario);
}
