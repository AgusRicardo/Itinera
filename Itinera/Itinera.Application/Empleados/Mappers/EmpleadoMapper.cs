using Itinera.Application.Empleados.Dtos;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Empleados.Mappers;

public static class EmpleadoMapper
{
    public static EmpleadoResponse ToResponse(Empleado empleado) => new()
    {
        Id = empleado.Id,
        Nombre = empleado.Nombre,
        Apellido = empleado.Apellido,
        Email = empleado.Email,
        Telefono = empleado.Telefono,
        Activo = empleado.Activo,
        CargoId = empleado.CargoId,
        CargoDescripcion = empleado.Cargo?.Descripcion ?? string.Empty,
        EmpresaId = empleado.EmpresaId,
        EmpresaRazonSocial = empleado.Empresa?.RazonSocial ?? string.Empty
    };

    public static List<EmpleadoResponse> ToResponse(IEnumerable<Empleado> empleados)
        => empleados.Select(ToResponse).ToList();
}
