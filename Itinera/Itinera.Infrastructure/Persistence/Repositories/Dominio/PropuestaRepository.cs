using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Domain.Common;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class PropuestaRepository(AppDbContext context) : IPropuestaRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Propuesta propuesta)
    {
        _context.Propuestas.Add(propuesta);
        return _context.SaveChangesAsync();
    }

    public Task<Propuesta?> GetByIdAsync(int id)
        => ConRelaciones().FirstOrDefaultAsync(propuesta => propuesta.Id == id);

    public Task<List<Propuesta>> GetAllAsync()
        => ConRelaciones()
            .OrderByDescending(propuesta => propuesta.FechaCreacion)
            .ToListAsync();

    public Task<Pagina<Propuesta>> GetPaginaAsync(ListarPropuestasRequest request)
    {
        var consulta = ConRelaciones();

        if (request.EstadoId is int estadoId)
            consulta = consulta.Where(propuesta => propuesta.EstadoPropuestaId == estadoId);

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim().ToLower();
            consulta = consulta.Where(propuesta =>
                (propuesta.Cliente.Nombre + " " + propuesta.Cliente.Apellido).ToLower().Contains(texto)
                || propuesta.Destinos.Any(item =>
                    (item.Destino.Ciudad.Nombre + ", " + item.Destino.Ciudad.Pais.Nombre)
                        .ToLower().Contains(texto)));
        }

        return consulta
            .OrderByDescending(propuesta => propuesta.FechaCreacion)
            .ThenByDescending(propuesta => propuesta.Id)
            .PaginarAsync(request);
    }

    public async Task<ResumenPropuestasResponse> GetResumenAsync()
    {
        var cantidades = await _context.Propuestas
            .GroupBy(propuesta => propuesta.EstadoPropuestaId)
            .Select(grupo => new { EstadoId = grupo.Key, Cantidad = grupo.Count() })
            .ToDictionaryAsync(grupo => grupo.EstadoId, grupo => grupo.Cantidad);

        return new ResumenPropuestasResponse
        {
            Total = cantidades.Values.Sum(),
            Borradores = cantidades.GetValueOrDefault((int)EstadoPropuesta.Borrador),
            Presentadas = cantidades.GetValueOrDefault((int)EstadoPropuesta.Presentada),
            Aceptadas = cantidades.GetValueOrDefault((int)EstadoPropuesta.Aceptada)
        };
    }

    public Task<List<Propuesta>> GetByClienteAsync(int clienteId)
        => ConRelaciones()
            .Where(propuesta => propuesta.ClienteId == clienteId)
            .OrderByDescending(propuesta => propuesta.FechaCreacion)
            .ToListAsync();

    public Task UpdateAsync(Propuesta propuesta)
    {
        _context.Entry(propuesta).State = EntityState.Modified;
        return _context.SaveChangesAsync();
    }

    private IQueryable<Propuesta> ConRelaciones()
        => _context.Propuestas
            .Include(propuesta => propuesta.Cliente)
            .Include(propuesta => propuesta.Empleado)
            .Include(propuesta => propuesta.Etiquetas)
                .ThenInclude(item => item.Etiqueta)
            .Include(propuesta => propuesta.Destinos)
                .ThenInclude(item => item.Destino)
                    .ThenInclude(destino => destino.Ciudad)
                        .ThenInclude(ciudad => ciudad.Pais)
            .Include(propuesta => propuesta.Actividades)
                .ThenInclude(item => item.Actividad)
            .Include(propuesta => propuesta.Itinerarios)
                .ThenInclude(itinerario => itinerario.Destino)
            .Include(propuesta => propuesta.Itinerarios)
                .ThenInclude(itinerario => itinerario.Actividades)
                    .ThenInclude(item => item.Actividad)
            .AsSplitQuery();
}
