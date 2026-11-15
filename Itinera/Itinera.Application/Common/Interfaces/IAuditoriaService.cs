using Itinera.Domain.Common;

namespace Itinera.Application.Common.Interfaces;

public interface IAuditoriaService
{
    Task RegistrarAsync(
        AccionAuditoria accion,
        string entidad,
        int entidadId,
        string? detalle = null,
        CancellationToken cancellationToken = default);
}
