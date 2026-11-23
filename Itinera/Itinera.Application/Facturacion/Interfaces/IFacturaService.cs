using Itinera.Application.Common.Paginacion;
using Itinera.Application.Facturacion.Dtos;

namespace Itinera.Application.Facturacion.Interfaces;

public interface IFacturaService
{
    Task<FacturaResponse> CrearAsync(CrearFacturaRequest request);
    Task<FacturaResponse> ObtenerPorIdAsync(int id);
    Task<List<FacturaResponse>> ObtenerTodasAsync();
    Task<Pagina<FacturaResponse>> ObtenerPaginaAsync(PaginacionRequest paginacion);
    Task<FacturaResponse> RegistrarPagoAsync(int id, RegistrarPagoRequest request);
    Task AnularAsync(int id);
}
