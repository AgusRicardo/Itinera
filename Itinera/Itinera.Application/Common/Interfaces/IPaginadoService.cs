using Itinera.Application.Common.Paginacion;

namespace Itinera.Application.Common.Interfaces;

public interface IPaginadoService<TResponse, in TFiltro>
    where TFiltro : PaginacionRequest
{
    Task<Pagina<TResponse>> ObtenerPaginaAsync(TFiltro filtro);
}
