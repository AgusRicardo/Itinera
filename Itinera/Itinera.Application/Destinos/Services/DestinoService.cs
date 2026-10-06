using Itinera.Application.Ciudades.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Destinos.Dtos;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Destinos.Mappers;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Destinos.Services;

public class DestinoService(
    IDestinoRepository destinoRepository,
    ICiudadRepository ciudadRepository) : IDestinoService
{
    private readonly IDestinoRepository _destinoRepository = destinoRepository;
    private readonly ICiudadRepository _ciudadRepository = ciudadRepository;

    public async Task<DestinoResponse> CrearAsync(CrearDestinoRequest request)
    {
        Ciudad ciudad = await ObtenerCiudadById(request.CiudadId);

        var destino = new Destino(ciudad);

        await _destinoRepository.AddAsync(destino);

        return DestinoMapper.ToResponse(destino);
    }

    public async Task<DestinoResponse> ObtenerPorIdAsync(int id)
    {
        var destino = await ObtenerDestinoById(id);

        return DestinoMapper.ToResponse(destino);
    }

    public async Task<List<DestinoResponse>> ObtenerTodosAsync()
    {
        var destinos = await _destinoRepository.GetAllAsync();

        return DestinoMapper.ToResponse(destinos);
    }

    public async Task<DestinoResponse> ActualizarAsync(int id, ActualizarDestinoRequest request)
    {
        var destino = await ObtenerDestinoById(id);
        Ciudad ciudad = await ObtenerCiudadById(request.CiudadId);

        destino.ActualizarDatos(ciudad);

        await _destinoRepository.UpdateAsync(destino);

        return DestinoMapper.ToResponse(destino);
    }

    public async Task EliminarAsync(int id)
    {
        var destino = await ObtenerDestinoById(id);

        destino.Desactivar();

        await _destinoRepository.DeleteAsync(destino);
    }

    private async Task<Destino> ObtenerDestinoById(int id)
    {
        Destino? destino = await _destinoRepository.GetByIdAsync(id);

        return destino ?? throw new DestinoNoEncontradoException(id);
    }

    private async Task<Ciudad> ObtenerCiudadById(int ciudadId)
    {
        Ciudad? ciudad = await _ciudadRepository.GetByIdAsync(ciudadId);

        return ciudad ?? throw new CiudadNoEncontradaException(ciudadId);
    }
}
