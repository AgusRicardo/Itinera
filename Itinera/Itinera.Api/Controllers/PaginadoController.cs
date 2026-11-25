using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers;

/// <summary>
/// Agrega el listado paginado. El permiso de lectura se declara en la clase que hereda.
/// </summary>
public abstract class PaginadoController<TResponse, TFiltro>(
    IPaginadoService<TResponse, TFiltro> servicio) : ApiController
    where TFiltro : PaginacionRequest
{
    [HttpGet("paginado")]
    public async Task<ActionResult<Pagina<TResponse>>> ObtenerPagina([FromQuery] TFiltro filtro)
        => Ok(await servicio.ObtenerPaginaAsync(filtro));
}
