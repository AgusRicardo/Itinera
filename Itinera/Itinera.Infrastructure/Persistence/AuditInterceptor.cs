using Itinera.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Itinera.Infrastructure.Persistence;

public sealed class AuditInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AplicarAuditoria(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AplicarAuditoria(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AplicarAuditoria(DbContext? context)
    {
        if (context is null)
            return;

        var ahora = DateTime.UtcNow;
        var usuarioId = currentUserService.UsuarioId ?? Guid.Empty;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            if (entry.State == EntityState.Added)
            {
                AsignarSiExiste(entry, "FechaRegistracion", ahora);
                AsignarSiExiste(entry, "UsuarioRegistracionId", usuarioId);
            }

            AsignarSiExiste(entry, "FechaModificacion", ahora);
            AsignarSiExiste(entry, "UsuarioModificacionId", usuarioId);
        }
    }

    private static void AsignarSiExiste(EntityEntry entry, string propiedad, object valor)
    {
        if (entry.Metadata.FindProperty(propiedad) is not null)
            entry.Property(propiedad).CurrentValue = valor;
    }
}
