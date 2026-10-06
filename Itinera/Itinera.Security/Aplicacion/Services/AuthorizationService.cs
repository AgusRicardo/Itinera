using Itinera.Security.Application.Interfaces;

namespace Itinera.Security.Application.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly ISecurityRepository _repository;

    public AuthorizationService(ISecurityRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> VerificarPermisoAsync(Guid usuarioId, string codigoPermiso)
    {
        return _repository.UsuarioTienePermisoAsync(usuarioId, codigoPermiso);
    }

    public async Task<List<string>> ObtenerPermisosAsync(Guid usuarioId)
    {
        var permisos = await _repository.GetPermisosDeUsuarioAsync(usuarioId);
        return permisos.Select(p => p.Codigo).ToList();
    }
}
