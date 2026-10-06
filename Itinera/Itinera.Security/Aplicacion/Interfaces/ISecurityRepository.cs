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
}
