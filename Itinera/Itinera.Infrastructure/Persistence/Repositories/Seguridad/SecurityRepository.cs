using Itinera.Security.Application.Interfaces;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Seguridad;

public class SecurityRepository(AppDbContext context) : ISecurityRepository
{
    private readonly AppDbContext _context = context;

    public Task<Usuario?> GetByEmailAsync(string email)
        => _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

    public Task<Usuario?> GetByIdAsync(Guid id)
        => _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<List<Rol>> GetRolesDeUsuarioAsync(Guid usuarioId)
    {
        var componentes = await ObtenerComponentesDeRolAsync(usuarioId);

        var rolesIds = await _context.UsuarioComponentRoles
            .Where(ucr => componentes.Contains(ucr.UsuarioComponentId))
            .Select(ucr => ucr.RolId)
            .Distinct()
            .ToListAsync();

        return await RolesConPermisos()
            .Where(r => rolesIds.Contains(r.Id))
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

    public Task<List<Usuario>> ListarUsuariosAsync()
        => _context.Usuarios.OrderBy(u => u.Nombre).ToListAsync();

    public Task<List<Rol>> ListarRolesAsync()
        => RolesConPermisos().OrderBy(r => r.Nombre).ToListAsync();

    public Task<Rol?> GetRolAsync(int id)
        => RolesConPermisos().FirstOrDefaultAsync(r => r.Id == id);

    public Task<List<Permiso>> ListarPermisosAsync()
        => _context.Permisos.OrderBy(p => p.Codigo).ToListAsync();

    public Task<Permiso?> GetPermisoAsync(int id)
        => _context.Permisos.FirstOrDefaultAsync(p => p.Id == id);

    public Task<List<GrupoUsuarios>> ListarGruposAsync()
        => GruposConMiembros()
            .OrderBy(g => g.Nombre)
            .ToListAsync();

    public async Task CrearRolAsync(Rol rol)
    {
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();
    }

    public async Task CrearGrupoAsync(GrupoUsuarios grupo)
    {
        _context.GruposUsuarios.Add(grupo);
        await _context.SaveChangesAsync();
    }

    public async Task AsignarRolAsync(Guid componenteId, int rolId)
    {
        var existe = await _context.UsuarioComponentRoles
            .AnyAsync(ucr => ucr.UsuarioComponentId == componenteId && ucr.RolId == rolId);
        if (existe)
            return;

        var componente = await _context.UsuariosSeguridad
            .FirstOrDefaultAsync(c => c.Id == componenteId)
            ?? throw new KeyNotFoundException("Componente de seguridad no encontrado.");
        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == rolId)
            ?? throw new KeyNotFoundException("Rol no encontrado.");

        _context.UsuarioComponentRoles.Add(new UsuarioComponentRol(componente, rol));
        await _context.SaveChangesAsync();
    }

    public async Task QuitarRolAsync(Guid componenteId, int rolId)
    {
        var asignacion = await _context.UsuarioComponentRoles
            .FirstOrDefaultAsync(ucr => ucr.UsuarioComponentId == componenteId && ucr.RolId == rolId);
        if (asignacion is null)
            return;

        _context.UsuarioComponentRoles.Remove(asignacion);
        await _context.SaveChangesAsync();
    }

    public async Task AgregarMiembroAsync(Guid grupoId, Guid miembroId)
    {
        if (grupoId == miembroId)
            throw new InvalidOperationException("Un grupo no puede contenerse a sí mismo.");

        if (await AlcanzaDescendienteAsync(miembroId, grupoId))
            throw new InvalidOperationException("La operación produciría un ciclo de grupos.");

        var grupo = await GruposConMiembros()
            .FirstOrDefaultAsync(g => g.Id == grupoId)
            ?? throw new KeyNotFoundException("Grupo no encontrado.");
        var miembro = await _context.UsuariosSeguridad
            .FirstOrDefaultAsync(c => c.Id == miembroId)
            ?? throw new KeyNotFoundException("Miembro no encontrado.");

        grupo.Agregar(miembro);
        await _context.SaveChangesAsync();
    }

    public async Task QuitarMiembroAsync(Guid grupoId, Guid miembroId)
    {
        var miembro = await _context.GrupoMiembros
            .FirstOrDefaultAsync(gm => gm.GrupoId == grupoId && gm.MiembroId == miembroId);
        if (miembro is null)
            return;

        _context.GrupoMiembros.Remove(miembro);
        await _context.SaveChangesAsync();
    }

    public Task GuardarAsync() => _context.SaveChangesAsync();

    private IQueryable<Rol> RolesConPermisos()
        => _context.Roles.Include(rol => rol.Permisos);

    private IQueryable<GrupoUsuarios> GruposConMiembros()
        => _context.GruposUsuarios.Include(grupo => grupo.GrupoMiembros);

    private async Task<List<Guid>> ObtenerComponentesDeRolAsync(Guid usuarioId)
    {
        var aristas = await ObtenerAristasAsync();
        var componentes = new HashSet<Guid> { usuarioId };
        var cola = new Queue<Guid>();
        cola.Enqueue(usuarioId);

        while (cola.Count > 0)
        {
            var actual = cola.Dequeue();

            foreach (var arista in aristas.Where(a => a.MiembroId == actual))
            {
                if (componentes.Add(arista.GrupoId))
                    cola.Enqueue(arista.GrupoId);
            }
        }

        return componentes.ToList();
    }

    private async Task<bool> AlcanzaDescendienteAsync(Guid origenId, Guid buscadoId)
    {
        var aristas = await ObtenerAristasAsync();
        var visitados = new HashSet<Guid>();
        var pila = new Stack<Guid>();
        pila.Push(origenId);

        while (pila.Count > 0)
        {
            var actual = pila.Pop();
            if (actual == buscadoId)
                return true;
            if (!visitados.Add(actual))
                continue;

            foreach (var arista in aristas.Where(a => a.GrupoId == actual))
                pila.Push(arista.MiembroId);
        }

        return false;
    }

    private async Task<List<(Guid GrupoId, Guid MiembroId)>> ObtenerAristasAsync()
    {
        var aristas = await _context.GrupoMiembros
            .Select(gm => new { gm.GrupoId, gm.MiembroId })
            .ToListAsync();

        return aristas.Select(a => (a.GrupoId, a.MiembroId)).ToList();
    }
}
