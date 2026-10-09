using Itinera.Application.Empleados.Interfaces;
using Itinera.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class EmpleadoRepository(AppDbContext context) : IEmpleadoRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Empleado empleado)
    {
        _context.Empleados.Add(empleado);
        return _context.SaveChangesAsync();
    }

    public Task<Empleado?> GetByIdAsync(int id)
        => ConRelaciones().FirstOrDefaultAsync(empleado => empleado.Id == id);

    public Task<List<Empleado>> GetAllAsync()
        => ConRelaciones().ToListAsync();

    private IQueryable<Empleado> ConRelaciones()
        => _context.Empleados
            .Include(empleado => empleado.Cargo)
            .Include(empleado => empleado.Empresa);

    public Task UpdateAsync(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
        return _context.SaveChangesAsync();
    }
}
