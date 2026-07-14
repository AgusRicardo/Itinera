namespace Itinera.Application.Seguridad.Dtos;

public record RegistroRequest(
    string Nombre,
    string Email,
    string Password);
