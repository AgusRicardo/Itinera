using Itinera.Application.Common.Interfaces;
using Itinera.Domain.Common;
using Itinera.Infrastructure.Persistence;

namespace Itinera.Infrastructure.Servicios;

public sealed class AuditoriaService(
    AppDbContext context,
    ICurrentUserService currentUserService,
    ITenantContext tenantContext) : IAuditoriaService
{
    public Task RegistrarAsync(
        AccionAuditoria accion,
        string entidad,
        int entidadId,
        string? detalle = null,
        CancellationToken cancellationToken = default)
    {
        var registro = new RegistroAuditoria(
            currentUserService.UsuarioId,
            tenantContext.EmpresaId,
            accion,
            entidad,
            entidadId,
            detalle);

        context.Set<RegistroAuditoria>().Add(registro);
        return context.SaveChangesAsync(cancellationToken);
    }
}
