using Itinera.Application.Common.Interfaces;
using Itinera.Domain.Catalogos;
using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Facturacion;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.Infrastructure.Persistence;

/// <summary>
/// Registra los cambios de catálogos, personas y seguridad. Propuestas, itinerarios y facturas
/// se registran en sus servicios, con la acción de negocio.
/// </summary>
public sealed class RegistroAuditoriaInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    private static readonly HashSet<Type> Auditadas =
    [
        typeof(Cliente), typeof(Empleado), typeof(EmpresaEntidad), typeof(Cargo),
        typeof(Pais), typeof(Ciudad), typeof(Destino), typeof(Actividad),
        typeof(Etiqueta), typeof(ConceptoFactura),
        typeof(Usuario), typeof(Rol), typeof(GrupoUsuarios),
        typeof(GrupoMiembro), typeof(UsuarioComponentRol), typeof(UsuarioEmpleado)
    ];

    private static readonly HashSet<string> TablasAuditadas = ["RolPermiso"];
    private static readonly HashSet<string> CamposTecnicos =
        ["FechaRegistracion", "UsuarioRegistracionId", "FechaModificacion", "UsuarioModificacionId"];

    private List<Cambio> _pendientes = [];

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is AppDbContext context && currentUserService.UsuarioId is not null)
        {
            context.ChangeTracker.DetectChanges();
            _pendientes = context.ChangeTracker.Entries()
                .Where(EsAuditada)
                .Select(CrearCambio)
                .OfType<Cambio>()
                .ToList();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is AppDbContext context && _pendientes.Count > 0)
        {
            var cambios = _pendientes;
            _pendientes = [];

            context.RegistrosAuditoria.AddRange(cambios.Select(cambio => new RegistroAuditoria(
                currentUserService.UsuarioId,
                context.EmpresaActualId,
                cambio.Accion,
                cambio.Entidad,
                cambio.EntidadId ?? ObtenerClave(cambio.Entrada),
                cambio.Detalle)));
            await context.SaveChangesAsync(cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _pendientes = [];

        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private static bool EsAuditada(EntityEntry entrada)
        => Auditadas.Contains(entrada.Metadata.ClrType) || TablasAuditadas.Contains(entrada.Metadata.Name);

    private static Cambio? CrearCambio(EntityEntry entrada)
    {
        var entidad = entrada.Metadata.ShortName();

        switch (entrada.State)
        {
            case EntityState.Added:
                return new Cambio(entrada, AccionAuditoria.Crear, entidad, null, null);
            case EntityState.Deleted:
                return new Cambio(entrada, AccionAuditoria.Eliminar, entidad, ObtenerClave(entrada), null);
            case EntityState.Modified:
                var campos = entrada.Properties
                    .Where(campo => !CamposTecnicos.Contains(campo.Metadata.Name)
                        && !Equals(campo.OriginalValue, campo.CurrentValue))
                    .ToList();
                if (campos.Count == 0)
                    return null;

                var activo = campos.FirstOrDefault(campo => campo.Metadata.Name == "Activo");
                var accion = activo is null
                    ? AccionAuditoria.Modificar
                    : activo.CurrentValue is true ? AccionAuditoria.Activar : AccionAuditoria.Desactivar;

                return new Cambio(
                    entrada,
                    accion,
                    entidad,
                    ObtenerClave(entrada),
                    string.Join(", ", campos.Select(campo => campo.Metadata.Name)));
            default:
                return null;
        }
    }

    private static string ObtenerClave(EntityEntry entrada)
        => string.Join("|", entrada.Metadata.FindPrimaryKey()!.Properties
            .Select(propiedad => entrada.Property(propiedad.Name).CurrentValue));

    private sealed record Cambio(
        EntityEntry Entrada,
        AccionAuditoria Accion,
        string Entidad,
        string? EntidadId,
        string? Detalle);
}
