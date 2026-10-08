using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Itinera.Security.Domain;

namespace Itinera.Security.Application.Services;

public class GrupoAdminService(ISecurityRepository repository) : IGrupoAdminService
{
    public async Task<List<GrupoAdminResponse>> ListarAsync()
    {
        var grupos = await repository.ListarGruposAsync();
        var resultado = new List<GrupoAdminResponse>(grupos.Count);

        foreach (var grupo in grupos)
        {
            var roles = await repository.GetRolesDeUsuarioAsync(grupo.Id);
            resultado.Add(new GrupoAdminResponse(
                grupo.Id,
                grupo.Nombre,
                grupo.GrupoMiembros.Select(gm => gm.MiembroId).ToList(),
                roles.Select(r => r.Nombre).ToList()));
        }

        return resultado;
    }

    public async Task<GrupoAdminResponse> CrearAsync(CrearGrupoRequest request)
    {
        var grupo = new GrupoUsuarios(request.Nombre);
        await repository.CrearGrupoAsync(grupo);

        return new GrupoAdminResponse(
            grupo.Id,
            grupo.Nombre,
            Array.Empty<Guid>(),
            Array.Empty<string>());
    }

    public Task AgregarMiembroAsync(Guid grupoId, Guid miembroId)
        => repository.AgregarMiembroAsync(grupoId, miembroId);

    public Task QuitarMiembroAsync(Guid grupoId, Guid miembroId)
        => repository.QuitarMiembroAsync(grupoId, miembroId);

    public Task AsignarRolAsync(Guid grupoId, int rolId)
        => repository.AsignarRolAsync(grupoId, rolId);

    public Task QuitarRolAsync(Guid grupoId, int rolId)
        => repository.QuitarRolAsync(grupoId, rolId);
}
