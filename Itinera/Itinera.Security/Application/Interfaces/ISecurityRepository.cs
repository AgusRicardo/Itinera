using Itinera.Security.Application.Dtos;
using Itinera.Security.Domain;

namespace Itinera.Security.Application.Interfaces;

public interface ISecurityRepository
{
    Task<Usuario> GetByEmailAsync(string email);
    Task<Usuario> GetByIdAsync(Guid id);
    Task<List<Rol>> GetRolesDeUsuarioAsync(Guid usuarioId);
    Task<List<Permiso>> GetPermisosDeUsuarioAsync(Guid usuarioId);
    Task<EmpresaDeUsuarioDto?> GetEmpresaDeUsuarioAsync(Guid usuarioId);
    Task<bool> ExisteRolAsync(string nombre);

    Task<List<Usuario>> BuscarUsuariosAsync(string texto);
    Task<List<Rol>> GetRolesAsync();
    Task<Rol> GetRolAsync(int id);
    Task<List<Permiso>> GetPermisosAsync();
    Task<Permiso> GetPermisoAsync(int id);
    Task<List<GrupoUsuarios>> GetGruposAsync();

    Task AddRolAsync(Rol rol);
    Task AddGrupoAsync(GrupoUsuarios grupo);
    Task AsignarRolAsync(Guid componenteId, int rolId);
    Task QuitarRolAsync(Guid componenteId, int rolId);
    Task AgregarMiembroAsync(Guid grupoId, Guid miembroId);
    Task QuitarMiembroAsync(Guid grupoId, Guid miembroId);
    Task SaveAsync();
}
