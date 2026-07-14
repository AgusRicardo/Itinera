namespace Itinera.Application.Seguridad.Dtos;

public record LoginResponse(
    string Token,
    Guid UsuarioId,
    string Nombre,
    string Email,
    IReadOnlyList<string> Roles);
