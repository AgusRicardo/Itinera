using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Facturacion.Dtos;

namespace Itinera.Application.Facturacion.Interfaces;

public interface IFacturaService : IPaginadoService<FacturaResponse, ListadoComercialRequest>
{
    Task<FacturaResponse> CrearAsync(CrearFacturaRequest request);
    Task<FacturaResponse> ObtenerPorIdAsync(int id);
    Task<List<FacturaResponse>> ObtenerTodosAsync();
    Task<FacturaResponse> RegistrarPagoAsync(int id, RegistrarPagoRequest request);
    Task AnularAsync(int id);
}
