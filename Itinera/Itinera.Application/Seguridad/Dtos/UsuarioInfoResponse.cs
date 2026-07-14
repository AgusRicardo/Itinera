namespace Itinera.Application.Seguridad.Dtos;

public record UsuarioInfoResponse(
    Guid UsuarioId,
    string Email,
    string Nombre,
    IReadOnlyList<string> Roles);
