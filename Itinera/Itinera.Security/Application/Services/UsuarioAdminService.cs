using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Itinera.Security.Domain;

namespace Itinera.Security.Application.Services;

public class UsuarioAdminService(ISecurityRepository repository) : IUsuarioAdminService
{
    public async Task<List<UsuarioAdminResponse>> BuscarAsync(string texto)
    {
        var usuarios = await repository.BuscarUsuariosAsync(texto);
        return await ConvertirAsync(usuarios);
    }

    public async Task<List<UsuarioAdminResponse>> ConvertirAsync(List<Usuario> usuarios)
    {
        var resultado = new List<UsuarioAdminResponse>(usuarios.Count);

        foreach (var usuario in usuarios)
        {
            var roles = await repository.GetRolesDeUsuarioAsync(usuario.Id);
            var empresa = await repository.GetEmpresaDeUsuarioAsync(usuario.Id);
            resultado.Add(new UsuarioAdminResponse(
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Activo,
                roles.Select(r => r.Nombre).ToList(),
                empresa?.EmpresaId,
                empresa?.RazonSocial ?? string.Empty,
                empresa?.Activa ?? false,
                empresa?.CargoId));
        }

        return resultado;
    }

    public Task AsignarRolAsync(Guid usuarioId, int rolId)
        => repository.AsignarRolAsync(usuarioId, rolId);

    public Task QuitarRolAsync(Guid usuarioId, int rolId)
        => repository.QuitarRolAsync(usuarioId, rolId);

    public Task ActivarAsync(Guid usuarioId) => CambiarActivoAsync(usuarioId, activo: true);

    public Task DesactivarAsync(Guid usuarioId) => CambiarActivoAsync(usuarioId, activo: false);

    private async Task CambiarActivoAsync(Guid usuarioId, bool activo)
    {
        var usuario = await repository.GetByIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        if (activo)
            usuario.Activar();
        else
            usuario.Desactivar();

        await repository.SaveAsync();
    }
}
