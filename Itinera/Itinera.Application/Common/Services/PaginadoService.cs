using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;

namespace Itinera.Application.Common.Services;

public abstract class PaginadoService<TEntidad, TResponse, TFiltro>(
    IPaginadoRepository<TEntidad, TFiltro> repositorio) : IPaginadoService<TResponse, TFiltro>
    where TFiltro : PaginacionRequest
{
    public async Task<Pagina<TResponse>> ObtenerPaginaAsync(TFiltro filtro)
    {
        var pagina = await repositorio.GetPaginaAsync(filtro);

        return pagina.Convertir(Convertir);
    }

    protected abstract List<TResponse> Convertir(IEnumerable<TEntidad> entidades);
}
