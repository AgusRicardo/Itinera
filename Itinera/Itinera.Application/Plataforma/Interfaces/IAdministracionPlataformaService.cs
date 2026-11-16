using Itinera.Application.Plataforma.Dtos;

namespace Itinera.Application.Plataforma.Interfaces;

public interface IAdministracionPlataformaService
{
    Task<UsuarioEmpresaResponse> CrearUsuarioEmpresaAsync(
        CrearUsuarioEmpresaRequest request,
        CancellationToken cancellationToken = default);

    Task AsignarUsuarioEmpresaAsync(
        Guid usuarioId,
        AsignarUsuarioEmpresaRequest request,
        CancellationToken cancellationToken = default);
}
