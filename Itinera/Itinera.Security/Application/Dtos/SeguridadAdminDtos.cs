using System.ComponentModel.DataAnnotations;

namespace Itinera.Security.Application.Dtos;

public sealed record UsuarioAdminResponse(
    Guid Id,
    string Nombre,
    string Email,
    bool Activo,
    IReadOnlyList<string> Roles,
    int? EmpresaId,
    string EmpresaRazonSocial,
    bool EmpresaActiva,
    int? CargoId);

public sealed record EmpresaDeUsuarioDto(int EmpresaId, string RazonSocial, bool Activa, int CargoId);

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
    IReadOnlyList<MiembroGrupoResponse> Miembros,
    IReadOnlyList<string> Roles);

public sealed record MiembroGrupoResponse(Guid Id, string Nombre);

public sealed record CrearRolRequest(
    [Required, MaxLength(100)] string Nombre,
    [Required, MaxLength(300)] string Descripcion,
    IReadOnlyList<string> Permisos);

public sealed record CrearGrupoRequest([Required, MaxLength(200)] string Nombre);

public sealed record AsignarRolRequest([Range(1, int.MaxValue)] int RolId);

public sealed record AsignarPermisoRequest([Range(1, int.MaxValue)] int PermisoId);

public sealed record AgregarMiembroRequest(Guid MiembroId);
