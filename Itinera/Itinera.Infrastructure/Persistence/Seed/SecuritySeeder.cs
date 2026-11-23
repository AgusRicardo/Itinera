using Itinera.Security.Application.Common;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Itinera.Infrastructure.Persistence.Seed;

public sealed class SecuritySeeder(
    AppDbContext context,
    IConfiguration configuration,
    ILogger<SecuritySeeder> logger)
{
    public async Task SembrarAsync(CancellationToken cancellationToken = default)
    {
        var permisos = await SembrarPermisosAsync(cancellationToken);
        await EliminarPermisosFueraDelCatalogoAsync(cancellationToken);
        var roles = await SembrarRolesAsync(permisos, cancellationToken);
        await SembrarAdministradorPlataformaAsync(roles, cancellationToken);
    }

    private async Task<Dictionary<string, Permiso>> SembrarPermisosAsync(CancellationToken cancellationToken)
    {
        var existentes = await context.Permisos.ToListAsync(cancellationToken);
        var porCodigo = existentes.ToDictionary(p => p.Codigo);

        foreach (var definicion in Permisos.Catalogo)
        {
            if (porCodigo.ContainsKey(definicion.Codigo))
                continue;

            var permiso = new Permiso(definicion.Codigo, definicion.Descripcion);
            context.Permisos.Add(permiso);
            porCodigo[definicion.Codigo] = permiso;
        }

        await context.SaveChangesAsync(cancellationToken);
        return porCodigo;
    }

    private async Task EliminarPermisosFueraDelCatalogoAsync(CancellationToken cancellationToken)
    {
        var codigosCatalogo = Permisos.Catalogo.Select(p => p.Codigo).ToList();

        var obsoletos = await context.Permisos
            .Where(p => !codigosCatalogo.Contains(p.Codigo))
            .ToListAsync(cancellationToken);

        if (obsoletos.Count == 0)
            return;

        var roles = await context.Roles
            .Include(rol => rol.Permisos)
            .ToListAsync(cancellationToken);

        foreach (var rol in roles)
        {
            foreach (var permiso in obsoletos)
                rol.QuitarPermiso(permiso);
        }

        context.Permisos.RemoveRange(obsoletos);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogWarning(
            "Permisos fuera del catálogo eliminados: {Codigos}.",
            string.Join(", ", obsoletos.Select(p => p.Codigo)));
    }

    private async Task<Dictionary<string, Rol>> SembrarRolesAsync(
        Dictionary<string, Permiso> permisos,
        CancellationToken cancellationToken)
    {
        var roles = await context.Roles
            .Include(rol => rol.Permisos)
            .ToListAsync(cancellationToken);

        var porNombre = roles.ToDictionary(rol => rol.Nombre);

        var administrador = ObtenerOCrearRol(
            porNombre, Roles.Administrador, "Administrador de la empresa: opera como agente y gestiona clientes");

        var administradorItinera = ObtenerOCrearRol(
            porNombre, Roles.AdministradorItinera, "Administrador de la plataforma: empresas, usuarios, roles y grupos");

        var agente = ObtenerOCrearRol(
            porNombre, Roles.Agente, "Gestión operativa de clientes, propuestas y facturación");

        var delAgente = ResolverPermisos(permisos, Roles.PermisosDelAgente);
        var delAdministrador = ResolverPermisos(permisos, Roles.PermisosDelAdministrador);
        var delAdministradorItinera = ResolverPermisos(permisos, Roles.PermisosDelAdministradorItinera);

        SincronizarPermisos(administradorItinera, delAdministradorItinera);
        SincronizarPermisos(administrador, delAdministrador);
        SincronizarPermisos(agente, delAgente);

        await context.SaveChangesAsync(cancellationToken);
        return porNombre;
    }

    private static List<Permiso> ResolverPermisos(
        Dictionary<string, Permiso> permisos,
        IEnumerable<string> codigos)
        => codigos.Select(codigo => permisos[codigo]).ToList();

    private async Task SembrarAdministradorPlataformaAsync(
        Dictionary<string, Rol> roles,
        CancellationToken cancellationToken)
    {
        var nombre = configuration["PlatformAdmin:Nombre"];
        var email = configuration["PlatformAdmin:Email"];
        var password = configuration["PlatformAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "No se configuró 'PlatformAdmin:*'; no se crea el administrador de plataforma.");
            return;
        }

        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (usuario is null)
        {
            usuario = new Usuario(
                string.IsNullOrWhiteSpace(nombre) ? "Administrador de Itinera" : nombre,
                email,
                PasswordHasher.Hash(password));

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync(cancellationToken);
        }

        var rolAdministradorItinera = roles[Roles.AdministradorItinera];

        var yaTieneRol = await context.UsuarioComponentRoles
            .AnyAsync(ucr => ucr.UsuarioComponentId == usuario.Id
                && ucr.RolId == rolAdministradorItinera.Id, cancellationToken);

        if (!yaTieneRol)
        {
            context.UsuarioComponentRoles.Add(
                new UsuarioComponentRol(usuario, rolAdministradorItinera));

            await context.SaveChangesAsync(cancellationToken);
        }

        await QuitarMembresiasOperativasAsync(usuario.Id, cancellationToken);
        logger.LogInformation("Administrador de plataforma configurado: {Email}", email);
    }

    private async Task QuitarMembresiasOperativasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        var aristas = await context.GrupoMiembros
            .Select(miembro => new { miembro.GrupoId, miembro.MiembroId })
            .ToListAsync(cancellationToken);

        var asignaciones = await context.UsuarioComponentRoles
            .Include(asignacion => asignacion.Rol)
                .ThenInclude(rol => rol.Permisos)
            .ToListAsync(cancellationToken);

        var gruposConPermisosOperativos = asignaciones
            .Where(asignacion =>
                asignacion.UsuarioComponentId != usuarioId
                && asignacion.Rol.Permisos.Any(permiso => !Roles.EsDePlataforma(permiso.Codigo)
                    && permiso.Codigo != Permisos.SeguridadGestionar))
            .Select(asignacion => asignacion.UsuarioComponentId)
            .ToHashSet();

        var membresiasDirectas = aristas
            .Where(arista => arista.MiembroId == usuarioId)
            .ToList();

        foreach (var membresia in membresiasDirectas)
        {
            var visitados = new HashSet<Guid>();
            var pendientes = new Stack<Guid>();
            pendientes.Push(membresia.GrupoId);
            var alcanzaGrupoOperativo = false;

            while (pendientes.Count > 0)
            {
                var grupoId = pendientes.Pop();
                if (!visitados.Add(grupoId))
                    continue;

                if (gruposConPermisosOperativos.Contains(grupoId))
                {
                    alcanzaGrupoOperativo = true;
                    break;
                }

                foreach (var padre in aristas.Where(arista => arista.MiembroId == grupoId))
                    pendientes.Push(padre.GrupoId);
            }

            if (alcanzaGrupoOperativo)
            {
                var entidad = await context.GrupoMiembros
                    .FirstAsync(
                        miembro => miembro.GrupoId == membresia.GrupoId
                            && miembro.MiembroId == usuarioId,
                        cancellationToken);

                context.GrupoMiembros.Remove(entidad);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private Rol ObtenerOCrearRol(Dictionary<string, Rol> roles, string nombre, string descripcion)
    {
        if (roles.TryGetValue(nombre, out var existente))
            return existente;

        var rol = new Rol(nombre, descripcion);
        context.Roles.Add(rol);
        roles[nombre] = rol;
        return rol;
    }

    private static void SincronizarPermisos(Rol rol, IEnumerable<Permiso> deseados)
    {
        var codigosDeseados = deseados.Select(p => p.Codigo).ToHashSet();

        foreach (var permiso in rol.Permisos.Where(p => !codigosDeseados.Contains(p.Codigo)).ToList())
            rol.QuitarPermiso(permiso);

        foreach (var permiso in deseados)
            rol.AgregarPermiso(permiso);
    }
}
