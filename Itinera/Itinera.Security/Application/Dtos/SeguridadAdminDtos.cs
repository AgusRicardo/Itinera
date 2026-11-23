using System.ComponentModel.DataAnnotations;

namespace Itinera.Security.Application.Dtos;

public sealed record UsuarioAdminResponse(
    Guid Id,
    string Nombre,
    string Email,
    bool Activo,
    IReadOnlyList<string> Roles,
    int? EmpresaId,
    string EmpresaRazonSocial);

public sealed record RolAdminResponse(
    int Id,
    string Nombre,
    string Descripcion,
    IReadOnlyList<string> Permisos);

public sealed record PermisoAdminResponse(
    int Id,
    string Codigo,
    string Descripcion);

public sealed record GrupoAdminResponse(
    Guid Id,
    string Nombre,
    IReadOnlyList<Guid> Miembros,
    IReadOnlyList<string> Roles);

public sealed record CrearRolRequest(
    [Required] string Nombre,
    [Required] string Descripcion,
    IReadOnlyList<string> Permisos);

public sealed record CrearGrupoRequest([Required] string Nombre);

public sealed record AsignarRolRequest([Range(1, int.MaxValue)] int RolId);

public sealed record AsignarPermisoRequest([Range(1, int.MaxValue)] int PermisoId);

public sealed record AgregarMiembroRequest(Guid MiembroId);
