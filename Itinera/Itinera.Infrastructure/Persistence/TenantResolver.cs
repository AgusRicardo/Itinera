using Itinera.Application.Common.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence;

public sealed class TenantResolver(
    ICurrentUserService currentUserService,
    AppDbContext context,
    ITenantContext tenantContext)
{
    public void Resolver()
    {
        tenantContext.EsAdministradorPlataforma =
            currentUserService.TienePermiso(Permisos.PlataformaGestionarEmpresas);

        var usuarioId = currentUserService.UsuarioId;
        if (usuarioId is null)
            return;

        var empleadoId = context.UsuariosEmpleados
            .AsNoTracking()
            .Where(ue => ue.UsuarioId == usuarioId && ue.Activo)
            .Select(ue => (int?)ue.EmpleadoId)
            .FirstOrDefault();

        if (empleadoId is null)
            return;

        tenantContext.EmpresaId = context.Empleados
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(e => e.Id == empleadoId)
            .Select(e => (int?)e.EmpresaId)
            .FirstOrDefault();
    }
}
