using Itinera.Security.Application.Dtos;

namespace Itinera.Security.Application.Interfaces;

public interface IGrupoAdminService
{
    Task<List<GrupoAdminResponse>> ListarAsync();
    Task<GrupoAdminResponse> CrearAsync(CrearGrupoRequest request);
    Task AgregarMiembroAsync(Guid grupoId, Guid miembroId);
    Task QuitarMiembroAsync(Guid grupoId, Guid miembroId);
    Task AsignarRolAsync(Guid grupoId, int rolId);
    Task QuitarRolAsync(Guid grupoId, int rolId);
}
