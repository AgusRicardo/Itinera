using Itinera.Application.EstadosFacturas.Dtos;
using Itinera.Application.EstadosFacturas.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.EstadosFacturas;

public class EstadosFacturasController(IEstadoFacturaService estadoFacturaService) : ApiController
{
    private readonly IEstadoFacturaService _estadoFacturaService = estadoFacturaService;

    [HttpGet]
    [ProducesResponseType(typeof(List<EstadoFacturaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EstadoFacturaResponse>>> ObtenerTodos()
    {
        var response = await _estadoFacturaService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EstadoFacturaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstadoFacturaResponse>> ObtenerPorId(int id)
    {
        var response = await _estadoFacturaService.ObtenerPorIdAsync(id);

        return Ok(response);
    }
}
