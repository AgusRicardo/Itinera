using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;

namespace Itinera.Infrastructure.Persistence.Repositories;

public abstract class PaginadoRepository<TEntidad, TFiltro> : IPaginadoRepository<TEntidad, TFiltro>
    where TFiltro : PaginacionRequest
{
    public Task<Pagina<TEntidad>> GetPaginaAsync(TFiltro filtro)
        => Ordenar(Filtrar(filtro), filtro).PaginarAsync(filtro);

    protected abstract IQueryable<TEntidad> Filtrar(TFiltro filtro);

    /// <summary>El orden no puede tener empates: tiene que terminar en el identificador.</summary>
    protected abstract IOrderedQueryable<TEntidad> Ordenar(IQueryable<TEntidad> consulta, TFiltro filtro);
}
