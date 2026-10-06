using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Clientes.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Clientes;

public class ClientesController(IClienteService clienteService) : ApiController
{
    private readonly IClienteService _clienteService = clienteService;

    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteResponse>> Crear(CrearClienteRequest request)
    {
        var response = await _clienteService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ClienteResponse>>> ObtenerTodos()
    {
        var response = await _clienteService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> ObtenerPorId(int id)
    {
        var response = await _clienteService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> Actualizar(int id, ActualizarClienteRequest request)
    {
        var response = await _clienteService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _clienteService.EliminarAsync(id);

        return NoContent();
    }
}
