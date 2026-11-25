using System.Security.Claims;
using Itinera.Security.Application.Common;
using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Itinera.Security.Domain;

namespace Itinera.Security.Application.Services;

public class RolAdminService(ISecurityRepository repository) : IRolAdminService
{
    public async Task<List<RolAdminResponse>> ObtenerTodosAsync()
    {
        var roles = await repository.GetRolesAsync();
        return roles.Select(Mapear).ToList();
    }

    public async Task<List<PermisoAdminResponse>> ObtenerPermisosAsync(ClaimsPrincipal usuario)
    {
        var permisos = await repository.GetPermisosAsync();
        var esAdministradorPlataforma = EsAdministradorPlataforma(usuario);

        return permisos
            .Where(p => esAdministradorPlataforma || !Roles.EsDePlataforma(p.Codigo))
            .Select(p => new PermisoAdminResponse(p.Id, p.Codigo, p.Descripcion))
            .ToList();
    }

    public async Task<RolAdminResponse> CrearAsync(CrearRolRequest request, ClaimsPrincipal usuario)
    {
        var nombre = request.Nombre.Trim();
        if (await repository.ExisteRolAsync(nombre))
            throw new InvalidOperationException("Ya existe un rol con ese nombre.");

        var rol = new Rol(nombre, request.Descripcion.Trim());
        var permisos = await repository.GetPermisosAsync();

        foreach (var codigo in request.Permisos)
        {
            var permiso = permisos.FirstOrDefault(p => p.Codigo == codigo)
                ?? throw new ArgumentException($"Permiso desconocido: {codigo}.");
            rol.AgregarPermiso(permiso);
        }

        ValidarPermisosDePlataforma(usuario, rol.Permisos);

        await repository.AddRolAsync(rol);
        return Mapear(rol);
    }

    public async Task AsignarPermisoAsync(int rolId, int permisoId, ClaimsPrincipal usuario)
    {
        var rol = await repository.GetRolAsync(rolId)
            ?? throw new KeyNotFoundException("Rol no encontrado.");
        var permiso = await repository.GetPermisoAsync(permisoId)
            ?? throw new KeyNotFoundException("Permiso no encontrado.");

        ValidarPermisosDePlataforma(usuario, new[] { permiso });

        rol.AgregarPermiso(permiso);
        await repository.SaveAsync();
    }

    public async Task QuitarPermisoAsync(int rolId, int permisoId, ClaimsPrincipal usuario)
    {
        var rol = await repository.GetRolAsync(rolId)
            ?? throw new KeyNotFoundException("Rol no encontrado.");
        var permiso = await repository.GetPermisoAsync(permisoId)
            ?? throw new KeyNotFoundException("Permiso no encontrado.");

        ValidarPermisosDePlataforma(usuario, new[] { permiso });

        rol.QuitarPermiso(permiso);
        await repository.SaveAsync();
    }

    private static bool EsAdministradorPlataforma(ClaimsPrincipal usuario)
        => usuario.HasClaim(Permisos.ClaimType, Permisos.PlataformaGestionarEmpresas);

    private static void ValidarPermisosDePlataforma(ClaimsPrincipal usuario, IEnumerable<Permiso> permisos)
    {
        if (EsAdministradorPlataforma(usuario))
            return;

        var dePlataforma = permisos.FirstOrDefault(p => Roles.EsDePlataforma(p.Codigo));
        if (dePlataforma is not null)
            throw new UnauthorizedAccessException(
                $"El permiso '{dePlataforma.Codigo}' solo puede asignarlo un administrador de plataforma.");
    }

    private static RolAdminResponse Mapear(Rol rol)
        => new(
            rol.Id,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.Select(p => p.Codigo).ToList());
}
