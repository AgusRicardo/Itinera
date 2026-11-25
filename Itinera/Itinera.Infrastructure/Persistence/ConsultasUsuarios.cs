using Itinera.Application.Common.Interfaces;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence;

internal static class ConsultasUsuarios
{
    public static IQueryable<Usuario> UsuariosVisiblesPara(this AppDbContext context, ITenantContext tenant)
        => context.Usuarios.Where(usuario =>
            tenant.EsAdministradorPlataforma
            || (tenant.EmpresaId != null && context.UsuariosEmpleados.Any(vinculo =>
                vinculo.UsuarioId == usuario.Id
                && vinculo.Activo
                && vinculo.Empleado.EmpresaId == tenant.EmpresaId)));

    public static Task<bool> EmailRegistradoAsync(
        this AppDbContext context,
        string email,
        CancellationToken cancellationToken = default)
        => context.Usuarios.AnyAsync(usuario => usuario.Email.ToLower() == email.Trim().ToLower(), cancellationToken);
}
