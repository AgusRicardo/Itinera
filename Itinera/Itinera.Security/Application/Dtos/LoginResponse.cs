namespace Itinera.Security.Application.Dtos;

public record LoginResponse(
    string Token,
    Guid UsuarioId,
    string Nombre,
    string Email,
    IReadOnlyList<string> Roles);
