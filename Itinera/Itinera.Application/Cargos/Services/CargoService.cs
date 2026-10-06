using Itinera.Application.Cargos.Dtos;
using Itinera.Application.Cargos.Interfaces;
using Itinera.Application.Cargos.Mappers;
using Itinera.Application.Common.Exceptions;
using Itinera.Domain.Empresa;

namespace Itinera.Application.Cargos.Services;

public class CargoService(ICargoRepository cargoRepository) : ICargoService
{
    private readonly ICargoRepository _cargoRepository = cargoRepository;

    public async Task<CargoResponse> CrearAsync(CrearCargoRequest request)
    {
        var cargo = new Cargo(request.Descripcion);

        await _cargoRepository.AddAsync(cargo);

        return CargoMapper.ToResponse(cargo);
    }

    public async Task<CargoResponse> ObtenerPorIdAsync(int id)
    {
        var cargo = await ObtenerCargoById(id);

        return CargoMapper.ToResponse(cargo);
    }

    public async Task<List<CargoResponse>> ObtenerTodosAsync()
    {
        var cargos = await _cargoRepository.GetAllAsync();

        return CargoMapper.ToResponse(cargos);
    }

    public async Task<CargoResponse> ActualizarAsync(int id, ActualizarCargoRequest request)
    {
        var cargo = await ObtenerCargoById(id);

        cargo.ActualizarDescripcion(request.Descripcion);

        await _cargoRepository.UpdateAsync(cargo);

        return CargoMapper.ToResponse(cargo);
    }

    public async Task EliminarAsync(int id)
    {
        var cargo = await ObtenerCargoById(id);

        cargo.Desactivar();

        await _cargoRepository.DeleteAsync(cargo);
    }

    private async Task<Cargo> ObtenerCargoById(int id)
    {
        Cargo? cargo = await _cargoRepository.GetByIdAsync(id);

        return cargo ?? throw new CargoNoEncontradoException(id);
    }
}
