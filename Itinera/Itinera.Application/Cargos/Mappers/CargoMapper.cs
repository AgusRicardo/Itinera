using Itinera.Application.Cargos.Dtos;
using Itinera.Domain.Empresa;

namespace Itinera.Application.Cargos.Mappers;

public static class CargoMapper
{
    public static CargoResponse ToResponse(Cargo cargo) => new()
    {
        Id = cargo.Id,
        Descripcion = cargo.Descripcion,
        Activo = cargo.Activo
    };

    public static List<CargoResponse> ToResponse(IEnumerable<Cargo> cargos)
        => cargos.Select(ToResponse).ToList();
}
