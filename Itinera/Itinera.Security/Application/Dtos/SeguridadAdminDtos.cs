namespace Itinera.Security.Application.Dtos;

public sealed record UsuarioAdminResponse(
    Guid Id,
    string Nombre,
    string Email,
    bool Activo,
    IReadOnlyList<string> Roles);

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
    string Nombre,
    string Descripcion,
    IReadOnlyList<string> Permisos);

public sealed record CrearGrupoRequest(string Nombre);

public sealed record AsignarRolRequest(int RolId);

public sealed record AsignarPermisoRequest(int PermisoId);

public sealed record AgregarMiembroRequest(Guid MiembroId);
