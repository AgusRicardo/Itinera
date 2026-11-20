using Itinera.Application.Etiquetas.Dtos;
using Itinera.Domain.Catalogos;

namespace Itinera.Application.Etiquetas.Mappers;

public static class EtiquetaMapper
{
    public static EtiquetaResponse ToResponse(Etiqueta etiqueta)
        => new()
        {
            Id = etiqueta.Id,
            Nombre = etiqueta.Nombre,
            EmpresaId = etiqueta.EmpresaId,
            Activo = etiqueta.Activo
        };

    public static List<EtiquetaResponse> ToResponse(IEnumerable<Etiqueta> etiquetas)
        => etiquetas.Select(ToResponse).ToList();
}
