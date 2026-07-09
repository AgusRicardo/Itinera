using Itinera.Application.Empleados.Interfaces;
using Itinera.Domain.Usuarios;

namespace Itinera.Infrastructure.Persistence.Repositories;

public class EmpleadoRepository(AppDbContext context) : IEmpleadoRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Empleado empleado)
    {
        throw new NotImplementedException();
    }

    public async Task<Empleado?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
