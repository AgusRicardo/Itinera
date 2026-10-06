using Itinera.Domain.Usuarios;

namespace Itinera.Application.Empleados.Interfaces;

public interface IEmpleadoRepository
{
    Task AddAsync(Empleado empleado);
    Task<Empleado?> GetByIdAsync(int id);
    Task<List<Empleado>> GetAllAsync();
    Task UpdateAsync(Empleado empleado);
    Task DeleteAsync(Empleado empleado);
}
