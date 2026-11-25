using Itinera.Application.Common.Services;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Paises.Dtos;
using Itinera.Application.Paises.Interfaces;
using Itinera.Application.Paises.Mappers;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Paises.Services;

public class PaisService(IPaisRepository paisRepository)
    : PaginadoService<Pais, PaisResponse, ListadoRequest>(paisRepository), IPaisService
{
    private readonly IPaisRepository _paisRepository = paisRepository;

    public async Task<PaisResponse> CrearAsync(CrearPaisRequest request)
    {
        await ValidarDuplicadoAsync(request.Nombre, null);

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

        await ValidarDuplicadoAsync(request.Nombre, id);

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

    protected override List<PaisResponse> Convertir(IEnumerable<Pais> entidades)
        => PaisMapper.ToResponse(entidades);

    private async Task ValidarDuplicadoAsync(string nombre, int? idExcluido)
    {
        if (await _paisRepository.ExisteAsync(nombre.Trim(), idExcluido))
            throw new InvalidOperationException("Ya existe un país con ese nombre.");
    }

    private async Task<Pais> ObtenerPaisById(int id)
    {
        Pais? pais = await _paisRepository.GetByIdAsync(id);

        return pais ?? throw new PaisNoEncontradoException(id);
    }
}
