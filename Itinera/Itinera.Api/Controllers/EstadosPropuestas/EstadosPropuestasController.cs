using Itinera.Application.EstadosPropuestas.Dtos;
using Itinera.Application.EstadosPropuestas.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.EstadosPropuestas;

[Authorize(Policy = Permisos.CatalogosVer)]
public class EstadosPropuestasController(IEstadoPropuestaService estadoPropuestaService) : ApiController
{
    private readonly IEstadoPropuestaService _estadoPropuestaService = estadoPropuestaService;

    [HttpGet]
    [ProducesResponseType(typeof(List<EstadoPropuestaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EstadoPropuestaResponse>>> ObtenerTodos()
    {
        var response = await _estadoPropuestaService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EstadoPropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstadoPropuestaResponse>> ObtenerPorId(int id)
    {
        var response = await _estadoPropuestaService.ObtenerPorIdAsync(id);

        return Ok(response);
    }
}
