using Itinera.Application.Common.Exceptions;
using Itinera.Application.EstadosPropuestas.Dtos;
using Itinera.Application.EstadosPropuestas.Interfaces;
using Itinera.Application.EstadosPropuestas.Mappers;
using Itinera.Domain.Common;

namespace Itinera.Application.EstadosPropuestas.Services;

public class EstadoPropuestaService(IEstadoPropuestaRepository estadoPropuestaRepository) : IEstadoPropuestaService
{
    private readonly IEstadoPropuestaRepository _estadoPropuestaRepository = estadoPropuestaRepository;

    public async Task<List<EstadoPropuestaResponse>> ObtenerTodosAsync()
    {
        var estados = await _estadoPropuestaRepository.GetAllAsync();

        return EstadoPropuestaMapper.ToResponse(estados);
    }

    public async Task<EstadoPropuestaResponse> ObtenerPorIdAsync(int id)
    {
        EstadoPropuestaCatalogo? estado = await _estadoPropuestaRepository.GetByIdAsync(id);

        return estado is null
            ? throw new EstadoPropuestaNoEncontradoException(id)
            : EstadoPropuestaMapper.ToResponse(estado);
    }
}
