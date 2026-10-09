using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;

namespace Itinera.Security.Application.Services;

public class UsuarioAdminService(ISecurityRepository repository) : IUsuarioAdminService
{
    public async Task<List<UsuarioAdminResponse>> ListarAsync()
    {
        var usuarios = await repository.ListarUsuariosAsync();
        var resultado = new List<UsuarioAdminResponse>(usuarios.Count);

        foreach (var usuario in usuarios)
        {
            var roles = await repository.GetRolesDeUsuarioAsync(usuario.Id);
            resultado.Add(new UsuarioAdminResponse(
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Activo,
                roles.Select(r => r.Nombre).ToList()));
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

        await repository.GuardarAsync();
    }
}
