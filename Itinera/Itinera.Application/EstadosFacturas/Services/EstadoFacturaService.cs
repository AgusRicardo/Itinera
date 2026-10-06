using Itinera.Application.Common.Exceptions;
using Itinera.Application.EstadosFacturas.Dtos;
using Itinera.Application.EstadosFacturas.Interfaces;
using Itinera.Application.EstadosFacturas.Mappers;
using Itinera.Domain.Common;

namespace Itinera.Application.EstadosFacturas.Services;

public class EstadoFacturaService(IEstadoFacturaRepository estadoFacturaRepository) : IEstadoFacturaService
{
    private readonly IEstadoFacturaRepository _estadoFacturaRepository = estadoFacturaRepository;

    public async Task<List<EstadoFacturaResponse>> ObtenerTodosAsync()
    {
        var estados = await _estadoFacturaRepository.GetAllAsync();

        return EstadoFacturaMapper.ToResponse(estados);
    }

    public async Task<EstadoFacturaResponse> ObtenerPorIdAsync(int id)
    {
        EstadoFacturaCatalogo? estado = await _estadoFacturaRepository.GetByIdAsync(id);

        return estado is null
            ? throw new EstadoFacturaNoEncontradaException(id)
            : EstadoFacturaMapper.ToResponse(estado);
    }
}
