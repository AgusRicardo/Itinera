namespace Itinera.Application.Plataforma.Dtos;

public sealed record UsuarioEmpresaResponse(
    Guid UsuarioId,
    int EmpresaId,
    int EmpleadoId,
    string Nombre,
    string Email,
    string Rol);
