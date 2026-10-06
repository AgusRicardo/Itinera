using Itinera.Application.Actividades.Dtos;
using Itinera.Application.Actividades.Interfaces;
using Itinera.Application.Actividades.Mappers;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Actividades.Services;

public class ActividadService(
    IActividadRepository actividadRepository,
    IDestinoRepository destinoRepository) : IActividadService
{
    private readonly IActividadRepository _actividadRepository = actividadRepository;
    private readonly IDestinoRepository _destinoRepository = destinoRepository;

    public async Task<ActividadResponse> CrearAsync(CrearActividadRequest request)
    {
        Destino destino = await ObtenerDestinoById(request.DestinoId);

        var actividad = new Actividad(
            request.Nombre,
            request.Descripcion,
            request.CostoBase,
            request.DuracionEstimada,
            destino);

        await _actividadRepository.AddAsync(actividad);

        return ActividadMapper.ToResponse(actividad);
    }

    public async Task<ActividadResponse> ObtenerPorIdAsync(int id)
    {
        var actividad = await ObtenerActividadById(id);

        return ActividadMapper.ToResponse(actividad);
    }

    public async Task<List<ActividadResponse>> ObtenerTodosAsync()
    {
        var actividades = await _actividadRepository.GetAllAsync();

        return ActividadMapper.ToResponse(actividades);
    }

    public async Task<ActividadResponse> ActualizarAsync(int id, ActualizarActividadRequest request)
    {
        var actividad = await ObtenerActividadById(id);
        Destino destino = await ObtenerDestinoById(request.DestinoId);

        actividad.ActualizarInformacion(
            request.Nombre,
            request.Descripcion,
            request.CostoBase,
            request.DuracionEstimada,
            destino);

        await _actividadRepository.UpdateAsync(actividad);

        return ActividadMapper.ToResponse(actividad);
    }

    public async Task EliminarAsync(int id)
    {
        var actividad = await ObtenerActividadById(id);

        actividad.Desactivar();

        await _actividadRepository.DeleteAsync(actividad);
    }

    private async Task<Actividad> ObtenerActividadById(int id)
    {
        Actividad? actividad = await _actividadRepository.GetByIdAsync(id);

        return actividad ?? throw new ActividadNoEncontradaException(id);
    }

    private async Task<Destino> ObtenerDestinoById(int destinoId)
    {
        Destino? destino = await _destinoRepository.GetByIdAsync(destinoId);

        return destino ?? throw new DestinoNoEncontradoException(destinoId);
    }
}
