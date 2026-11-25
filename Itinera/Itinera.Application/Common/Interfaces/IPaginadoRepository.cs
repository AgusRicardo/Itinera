using Itinera.Application.Common.Paginacion;

namespace Itinera.Application.Common.Interfaces;

public interface IPaginadoRepository<TEntidad, in TFiltro>
    where TFiltro : PaginacionRequest
{
    Task<Pagina<TEntidad>> GetPaginaAsync(TFiltro filtro);
}
