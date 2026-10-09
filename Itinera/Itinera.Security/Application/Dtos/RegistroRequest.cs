namespace Itinera.Security.Application.Dtos;

public record RegistroRequest(
    string Nombre,
    string Email,
    string Password);
