using Itinera.Security.Domain;

namespace Itinera.Security.Application.Interfaces;

public interface ISecurityRepository
{
    Task<Usuario> GetByEmailAsync(string email);
    Task<Usuario> GetByIdAsync(Guid id);
    Task<List<Rol>> GetRolesDeUsuarioAsync(Guid usuarioId);
    Task<List<Permiso>> GetPermisosDeUsuarioAsync(Guid usuarioId);
    Task<Guid> CreateUsuarioAsync(Usuario usuario);
    Task<bool> UsuarioTienePermisoAsync(Guid usuarioId, string codigoPermiso);

    Task<List<Usuario>> ListarUsuariosAsync();
    Task<List<Rol>> ListarRolesAsync();
    Task<Rol> GetRolAsync(int id);
    Task<List<Permiso>> ListarPermisosAsync();
    Task<Permiso> GetPermisoAsync(int id);
    Task<List<GrupoUsuarios>> ListarGruposAsync();

    Task CrearRolAsync(Rol rol);
    Task CrearGrupoAsync(GrupoUsuarios grupo);
    Task AsignarRolAsync(Guid componenteId, int rolId);
    Task QuitarRolAsync(Guid componenteId, int rolId);
    Task AgregarMiembroAsync(Guid grupoId, Guid miembroId);
    Task QuitarMiembroAsync(Guid grupoId, Guid miembroId);
    Task GuardarAsync();
}
