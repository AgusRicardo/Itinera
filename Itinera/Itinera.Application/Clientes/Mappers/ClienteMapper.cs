using Itinera.Application.Clientes.Dtos;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Clientes.Mappers;

public static class ClienteMapper
{
    public static ClienteResponse ToResponse(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Nombre = cliente.Nombre,
        Apellido = cliente.Apellido,
        Email = cliente.Email,
        Telefono = cliente.Telefono,
        Activo = cliente.Activo
    };

    public static List<ClienteResponse> ToResponse(IEnumerable<Cliente> clientes)
        => clientes.Select(ToResponse).ToList();
}
