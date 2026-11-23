using Itinera.Application.Common.Paginacion;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence;

internal static class ConsultasPaginadas
{
    /// <summary>
    /// La consulta debe llegar con un orden sin empates. Si la página pedida ya no existe
    /// (por ejemplo, se borró su único elemento) devuelve la última.
    /// </summary>
    public static async Task<Pagina<T>> PaginarAsync<T>(
        this IOrderedQueryable<T> consulta,
        PaginacionRequest paginacion)
    {
        var total = await consulta.CountAsync();
        var ultimaPagina = Math.Max(1, (int)Math.Ceiling(total / (double)paginacion.TamanoPagina));
        var numeroPagina = Math.Min(paginacion.Pagina, ultimaPagina);
        var elementos = await consulta
            .Skip((numeroPagina - 1) * paginacion.TamanoPagina)
            .Take(paginacion.TamanoPagina)
            .ToListAsync();

        return new Pagina<T>(elementos, numeroPagina, paginacion.TamanoPagina, total);
    }
}
