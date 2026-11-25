using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Empresas.Dtos;

namespace Itinera.Application.Empresas.Interfaces;

public interface IEmpresaService : IPaginadoService<EmpresaResponse, ListadoActivableRequest>
{
    Task<EmpresaResponse> CrearAsync(CrearEmpresaRequest request);
    Task<EmpresaResponse> ObtenerPorIdAsync(int id);
    Task<List<EmpresaResponse>> ObtenerTodosAsync();
    Task<EmpresaResponse> ActualizarAsync(int id, ActualizarEmpresaRequest request);
    Task EliminarAsync(int id);
}
