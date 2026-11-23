using Itinera.Application.Common.Paginacion;

namespace Itinera.Application.Clientes.Dtos;

public class ListarClientesRequest : PaginacionRequest
{
    public string? Texto { get; set; }
    public bool? Activo { get; set; }
}
