using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Propuestas;

public class PropuestasController(IPropuestaService propuestaService) : ApiController
{
    private readonly IPropuestaService _propuestaService = propuestaService;

    [HttpPost]
    public async Task<ActionResult<CrearPropuestaResponse>> Crear(CrearPropuestaRequest request)
    {
        var response = await _propuestaService.CrearAsync(request);

        //return CreatedAtAction(
        //    nameof(Crear),
        //    new { id = response.Id },
        //    response);
        return Ok(response);
    }
}
