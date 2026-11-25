using Itinera.Application.Common.Paginacion;

namespace Itinera.Application.Actividades.Dtos;

public class ListarActividadesRequest : ListadoRequest
{
    public int? DestinoId { get; set; }
    public bool? GeneradoPorIA { get; set; }
}
