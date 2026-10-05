using Itinera.Security.Application.Interfaces;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Seguridad;

public class SecurityRepository(AppDbContext context) : ISecurityRepository
{
    private readonly AppDbContext _context = context;

    public Task<Usuario?> GetByEmailAsync(string email)
        => _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);

    public Task<Usuario?> GetByIdAsync(Guid id)
        => _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<List<Rol>> GetRolesDeUsuarioAsync(Guid usuarioId)
    {
        List<int> rolesDirectos = await RolesByUsuarioId(usuarioId);
        List<Guid> gruposDelUsuario = await GruposByUsuarioId(usuarioId);

        var rolesDeGrupos = new List<int>();

        if (gruposDelUsuario.Any())
        {
            rolesDeGrupos = await RolesDelGrupoByGrupo(gruposDelUsuario, rolesDeGrupos);
        }

        var todosLosRolesIds = rolesDirectos.Union(rolesDeGrupos).ToList();

        return await _context.Roles
            .Include(r => r.Permisos)
            .Where(r => todosLosRolesIds.Contains(r.Id))
            .ToListAsync();
    }

    private async Task<List<int>> RolesDelGrupoByGrupo(List<Guid> gruposDelUsuario, List<int> rolesDeGrupos)
    {
        rolesDeGrupos = await _context.UsuarioComponentRoles
                        .Where(ucr => gruposDelUsuario.Contains(ucr.UsuarioComponentId))
                        .Select(ucr => ucr.RolId)
                        .ToListAsync();
        return rolesDeGrupos;
    }

    private Task<List<Guid>> GruposByUsuarioId(Guid usuarioId)
    {
        return _context.GrupoMiembros
                    .Where(gm => gm.MiembroId == usuarioId)
                    .Select(gm => gm.GrupoId)
                    .ToListAsync();
    }

    private Task<List<int>> RolesByUsuarioId(Guid usuarioId)
    {
        return _context.UsuarioComponentRoles
                    .Where(ucr => ucr.UsuarioComponentId == usuarioId)
                    .Select(ucr => ucr.RolId)
                    .ToListAsync();
    }

    public async Task<List<Permiso>> GetPermisosDeUsuarioAsync(Guid usuarioId)
    {
        var roles = await GetRolesDeUsuarioAsync(usuarioId);
        return roles.SelectMany(r => r.Permisos).Distinct().ToList();
    }

    public async Task<Guid> CreateUsuarioAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario.Id;
    }

    public async Task<bool> UsuarioTienePermisoAsync(Guid usuarioId, string codigoPermiso)
    {
        var permisos = await GetPermisosDeUsuarioAsync(usuarioId);
        return permisos.Any(p => p.Codigo == codigoPermiso);
    }
}
