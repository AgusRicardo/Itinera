using Itinera.Application.Empresas.Dtos;
using Itinera.Domain.Empresa;

namespace Itinera.Application.Empresas.Mappers;

public static class EmpresaMapper
{
    public static EmpresaResponse ToResponse(Empresa empresa) => new()
    {
        Id = empresa.Id,
        RazonSocial = empresa.RazonSocial,
        CUIT = empresa.CUIT,
        Telefono = empresa.Telefono,
        Activo = empresa.Activo
    };

    public static List<EmpresaResponse> ToResponse(IEnumerable<Empresa> empresas)
        => empresas.Select(ToResponse).ToList();
}
