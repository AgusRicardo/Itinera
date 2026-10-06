using Itinera.Application.Common.Exceptions;
using Itinera.Application.Paises.Dtos;
using Itinera.Application.Paises.Interfaces;
using Itinera.Application.Paises.Mappers;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Paises.Services;

public class PaisService(IPaisRepository paisRepository) : IPaisService
{
    private readonly IPaisRepository _paisRepository = paisRepository;

    public async Task<PaisResponse> CrearAsync(CrearPaisRequest request)
    {
        var pais = new Pais(request.Nombre);

        await _paisRepository.AddAsync(pais);

        return PaisMapper.ToResponse(pais);
    }

    public async Task<PaisResponse> ObtenerPorIdAsync(int id)
    {
        var pais = await ObtenerPaisById(id);

        return PaisMapper.ToResponse(pais);
    }

    public async Task<List<PaisResponse>> ObtenerTodosAsync()
    {
        var paises = await _paisRepository.GetAllAsync();

        return PaisMapper.ToResponse(paises);
    }

    public async Task<PaisResponse> ActualizarAsync(int id, ActualizarPaisRequest request)
    {
        var pais = await ObtenerPaisById(id);

        pais.ActualizarNombre(request.Nombre);

        await _paisRepository.UpdateAsync(pais);

        return PaisMapper.ToResponse(pais);
    }

    public async Task EliminarAsync(int id)
    {
        var pais = await ObtenerPaisById(id);

        pais.Desactivar();

        await _paisRepository.DeleteAsync(pais);
    }

    private async Task<Pais> ObtenerPaisById(int id)
    {
        Pais? pais = await _paisRepository.GetByIdAsync(id);

        return pais ?? throw new PaisNoEncontradoException(id);
    }
}
