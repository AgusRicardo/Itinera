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
        var roles = await SembrarRolesAsync(permisos, cancellationToken);
        await SembrarAdministradorAsync(roles, cancellationToken);
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

    private async Task<Dictionary<string, Rol>> SembrarRolesAsync(
        Dictionary<string, Permiso> permisos,
        CancellationToken cancellationToken)
    {
        var roles = await context.Roles
            .Include(rol => rol.Permisos)
            .ToListAsync(cancellationToken);

        var porNombre = roles.ToDictionary(rol => rol.Nombre);

        var administrador = ObtenerOCrearRol(
            porNombre, Roles.Administrador, "Acceso total al sistema");

        var agente = ObtenerOCrearRol(
            porNombre, Roles.Agente, "Gestión operativa de clientes, propuestas y facturación");

        foreach (var permiso in permisos.Values)
            administrador.AgregarPermiso(permiso);

        foreach (var codigo in Roles.PermisosDelAgente)
            agente.AgregarPermiso(permisos[codigo]);

        await context.SaveChangesAsync(cancellationToken);
        return porNombre;
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

    private async Task SembrarAdministradorAsync(
        Dictionary<string, Rol> roles,
        CancellationToken cancellationToken)
    {
        if (await context.Usuarios.AnyAsync(cancellationToken))
            return;

        var nombre = configuration["Seed:Admin:Nombre"];
        var email = configuration["Seed:Admin:Email"];
        var password = configuration["Seed:Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "No se configuró 'Seed:Admin:*'; no se crea el usuario administrador inicial.");
            return;
        }

        var usuario = new Usuario(
            string.IsNullOrWhiteSpace(nombre) ? "Administrador" : nombre,
            email,
            PasswordHasher.Hash(password));

        context.Usuarios.Add(usuario);
        context.UsuarioComponentRoles.Add(
            new UsuarioComponentRol(usuario, roles[Roles.Administrador]));

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Usuario administrador inicial creado: {Email}", email);
    }
}
