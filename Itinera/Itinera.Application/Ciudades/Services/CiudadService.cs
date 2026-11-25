using Itinera.Application.Common.Services;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Ciudades.Dtos;
using Itinera.Application.Ciudades.Interfaces;
using Itinera.Application.Ciudades.Mappers;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Paises.Interfaces;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Ciudades.Services;

public class CiudadService(
    ICiudadRepository ciudadRepository,
    IPaisRepository paisRepository)
    : PaginadoService<Ciudad, CiudadResponse, ListadoRequest>(ciudadRepository), ICiudadService
{
    private readonly ICiudadRepository _ciudadRepository = ciudadRepository;
    private readonly IPaisRepository _paisRepository = paisRepository;

    public async Task<CiudadResponse> CrearAsync(CrearCiudadRequest request)
    {
        Pais pais = await ObtenerPaisById(request.PaisId);

        await ValidarDuplicadoAsync(request.Nombre, request.PaisId, null);

        var ciudad = new Ciudad(request.Nombre, pais);

        await _ciudadRepository.AddAsync(ciudad);

        return CiudadMapper.ToResponse(ciudad);
    }

    public async Task<CiudadResponse> ObtenerPorIdAsync(int id)
    {
        var ciudad = await ObtenerCiudadById(id);

        return CiudadMapper.ToResponse(ciudad);
    }

    public async Task<List<CiudadResponse>> ObtenerTodosAsync()
    {
        var ciudades = await _ciudadRepository.GetAllAsync();

        return CiudadMapper.ToResponse(ciudades);
    }

    public async Task<CiudadResponse> ActualizarAsync(int id, ActualizarCiudadRequest request)
    {
        var ciudad = await ObtenerCiudadById(id);
        Pais pais = await ObtenerPaisById(request.PaisId);

        await ValidarDuplicadoAsync(request.Nombre, request.PaisId, id);

        ciudad.ActualizarDatos(request.Nombre, pais);

        await _ciudadRepository.UpdateAsync(ciudad);

        return CiudadMapper.ToResponse(ciudad);
    }

    public async Task EliminarAsync(int id)
    {
        var ciudad = await ObtenerCiudadById(id);

        ciudad.Desactivar();

        await _ciudadRepository.DeleteAsync(ciudad);
    }

    protected override List<CiudadResponse> Convertir(IEnumerable<Ciudad> entidades)
        => CiudadMapper.ToResponse(entidades);

    private async Task ValidarDuplicadoAsync(string nombre, int paisId, int? idExcluido)
    {
        if (await _ciudadRepository.ExisteAsync(nombre.Trim(), paisId, idExcluido))
            throw new InvalidOperationException("Ya existe una ciudad con ese nombre en el país.");
    }

    private async Task<Ciudad> ObtenerCiudadById(int id)
    {
        Ciudad? ciudad = await _ciudadRepository.GetByIdAsync(id);

        return ciudad ?? throw new CiudadNoEncontradaException(id);
    }

    private async Task<Pais> ObtenerPaisById(int paisId)
    {
        Pais? pais = await _paisRepository.GetByIdAsync(paisId);

        return pais ?? throw new PaisNoEncontradoException(paisId);
    }
}
