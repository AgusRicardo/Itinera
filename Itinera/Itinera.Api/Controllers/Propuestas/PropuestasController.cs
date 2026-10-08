using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Propuestas;

[Authorize]
public class PropuestasController(IPropuestaService propuestaService) : ApiController
{
    private readonly IPropuestaService _propuestaService = propuestaService;

    [HttpPost]
    [Authorize(Policy = Permisos.PropuestasCrear)]
    public async Task<ActionResult<CrearPropuestaResponse>> Crear(CrearPropuestaRequest request)
    {
        var response = await _propuestaService.CrearAsync(request);

        return Ok(response);
    }
}
