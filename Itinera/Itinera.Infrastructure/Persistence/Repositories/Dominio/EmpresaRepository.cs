using Itinera.Application.Empresas.Interfaces;
using Itinera.Domain.Empresa;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class EmpresaRepository(AppDbContext context) : IEmpresaRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Empresa empresa)
    {
        _context.Empresas.Add(empresa);
        return _context.SaveChangesAsync();
    }

    public Task<Empresa?> GetByIdAsync(int id)
        => _context.Empresas.FindAsync(id).AsTask();

    public Task<List<Empresa>> GetAllAsync()
        => _context.Empresas.ToListAsync();

    public Task UpdateAsync(Empresa empresa)
    {
        _context.Empresas.Update(empresa);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Empresa empresa)
    {
        _context.Empresas.Update(empresa);
        return _context.SaveChangesAsync();
    }
}
