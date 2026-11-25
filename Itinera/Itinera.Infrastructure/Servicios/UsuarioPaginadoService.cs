using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Infrastructure.Persistence;
using Itinera.Infrastructure.Persistence.Repositories;
using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Servicios;

public sealed class UsuarioPaginadoService(
    AppDbContext context,
    ITenantContext tenantContext,
    IUsuarioAdminService usuarioAdminService)
    : PaginadoRepository<Usuario, ListadoActivableRequest>,
        IPaginadoService<UsuarioAdminResponse, ListadoActivableRequest>
{
    public async Task<Pagina<UsuarioAdminResponse>> ObtenerPaginaAsync(ListadoActivableRequest filtro)
    {
        var pagina = await GetPaginaAsync(filtro);

        return new Pagina<UsuarioAdminResponse>(
            await usuarioAdminService.ConvertirAsync(pagina.Elementos),
            pagina.NumeroPagina,
            pagina.TamanoPagina,
            pagina.Total);
    }

    protected override IQueryable<Usuario> Filtrar(ListadoActivableRequest filtro)
    {
        var consulta = context.UsuariosVisiblesPara(tenantContext);

        if (filtro.Activo is bool activo)
            consulta = consulta.Where(usuario => usuario.Activo == activo);

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(usuario =>
                AppDbContext.SinTildes(usuario.Nombre).Contains(texto)
                || AppDbContext.SinTildes(usuario.Email).Contains(texto)
                || context.UsuariosEmpleados.Any(vinculo =>
                    vinculo.UsuarioId == usuario.Id
                    && vinculo.Activo
                    && AppDbContext.SinTildes(vinculo.Empleado.Empresa.RazonSocial).Contains(texto))
                || context.UsuarioComponentRoles.Any(asignacion =>
                    asignacion.UsuarioComponentId == usuario.Id
                    && context.Roles.Any(rol =>
                        rol.Id == asignacion.RolId
                        && AppDbContext.SinTildes(rol.Nombre).Contains(texto))));

        return consulta;
    }

    protected override IOrderedQueryable<Usuario> Ordenar(
        IQueryable<Usuario> consulta,
        ListadoActivableRequest filtro)
    {
        var ordenada = filtro.Orden switch
        {
            "email" => consulta.OrdenarPor(usuario => usuario.Email, filtro.Descendente),
            "empresa" => consulta.OrdenarPor(
                usuario => context.UsuariosEmpleados
                    .Where(vinculo => vinculo.UsuarioId == usuario.Id && vinculo.Activo)
                    .Select(vinculo => vinculo.Empleado.Empresa.RazonSocial)
                    .FirstOrDefault(),
                filtro.Descendente),
            "estado" => consulta.OrdenarPor(usuario => usuario.Activo, filtro.Descendente),
            _ => consulta.OrdenarPor(usuario => usuario.Nombre, filtro.Descendente)
        };

        return ordenada.ThenBy(usuario => usuario.Id);
    }
}
