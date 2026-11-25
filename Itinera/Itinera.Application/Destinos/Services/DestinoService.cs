using Itinera.Application.Common.Services;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Ciudades.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Destinos.Dtos;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Destinos.Mappers;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Destinos.Services;

public class DestinoService(
    IDestinoRepository destinoRepository,
    ICiudadRepository ciudadRepository)
    : PaginadoService<Destino, DestinoResponse, ListadoRequest>(destinoRepository), IDestinoService
{
    private readonly IDestinoRepository _destinoRepository = destinoRepository;
    private readonly ICiudadRepository _ciudadRepository = ciudadRepository;

    public async Task<DestinoResponse> CrearAsync(CrearDestinoRequest request)
    {
        Ciudad ciudad = await ObtenerCiudadById(request.CiudadId);

        await ValidarDuplicadoAsync(request.CiudadId, null);

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

        await ValidarDuplicadoAsync(request.CiudadId, id);

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

    protected override List<DestinoResponse> Convertir(IEnumerable<Destino> entidades)
        => DestinoMapper.ToResponse(entidades);

    private async Task ValidarDuplicadoAsync(int ciudadId, int? idExcluido)
    {
        if (await _destinoRepository.ExisteAsync(ciudadId, idExcluido))
            throw new InvalidOperationException("Ya existe un destino para esa ciudad.");
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
