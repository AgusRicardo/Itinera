using Itinera.Application.Empleados.Dtos;

namespace Itinera.Application.Empleados.Interfaces;

public interface IEmpleadoService
{
    Task<EmpleadoResponse> CrearAsync(CrearEmpleadoRequest request);
    Task<EmpleadoResponse> ObtenerPorIdAsync(int id);
    Task<List<EmpleadoResponse>> ObtenerTodosAsync();
    Task<EmpleadoResponse> ActualizarAsync(int id, ActualizarEmpleadoRequest request);
    Task EliminarAsync(int id);
}
