using Itinera.Security.Application.Dtos;

namespace Itinera.Security.Application.Interfaces;

public interface IUsuarioAdminService
{
    Task<List<UsuarioAdminResponse>> ListarAsync();
    Task AsignarRolAsync(Guid usuarioId, int rolId);
    Task QuitarRolAsync(Guid usuarioId, int rolId);
    Task ActivarAsync(Guid usuarioId);
    Task DesactivarAsync(Guid usuarioId);
}
