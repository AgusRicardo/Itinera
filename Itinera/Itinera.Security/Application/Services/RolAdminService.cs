using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Itinera.Security.Domain;

namespace Itinera.Security.Application.Services;

public class RolAdminService(ISecurityRepository repository) : IRolAdminService
{
    public async Task<List<RolAdminResponse>> ListarAsync()
    {
        var roles = await repository.ListarRolesAsync();
        return roles.Select(Mapear).ToList();
    }

    public async Task<List<PermisoAdminResponse>> ListarPermisosAsync()
    {
        var permisos = await repository.ListarPermisosAsync();
        return permisos
            .Select(p => new PermisoAdminResponse(p.Id, p.Codigo, p.Descripcion))
            .ToList();
    }

    public async Task<RolAdminResponse> CrearAsync(CrearRolRequest request)
    {
        var rol = new Rol(request.Nombre, request.Descripcion);
        var permisos = await repository.ListarPermisosAsync();

        foreach (var codigo in request.Permisos)
        {
            var permiso = permisos.FirstOrDefault(p => p.Codigo == codigo)
                ?? throw new ArgumentException($"Permiso desconocido: {codigo}.");
            rol.AgregarPermiso(permiso);
        }

        await repository.CrearRolAsync(rol);
        return Mapear(rol);
    }

    public async Task AsignarPermisoAsync(int rolId, int permisoId)
    {
        var rol = await repository.GetRolAsync(rolId)
            ?? throw new KeyNotFoundException("Rol no encontrado.");
        var permiso = await repository.GetPermisoAsync(permisoId)
            ?? throw new KeyNotFoundException("Permiso no encontrado.");

        rol.AgregarPermiso(permiso);
        await repository.GuardarAsync();
    }

    public async Task QuitarPermisoAsync(int rolId, int permisoId)
    {
        var rol = await repository.GetRolAsync(rolId)
            ?? throw new KeyNotFoundException("Rol no encontrado.");
        var permiso = await repository.GetPermisoAsync(permisoId)
            ?? throw new KeyNotFoundException("Permiso no encontrado.");

        rol.QuitarPermiso(permiso);
        await repository.GuardarAsync();
    }

    private static RolAdminResponse Mapear(Rol rol)
        => new(
            rol.Id,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.Select(p => p.Codigo).ToList());
}
