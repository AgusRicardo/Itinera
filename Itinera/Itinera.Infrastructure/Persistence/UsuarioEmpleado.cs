using Itinera.Domain.Usuarios;
using Itinera.Security.Domain;

namespace Itinera.Infrastructure.Persistence;

public class UsuarioEmpleado
{
    public Guid UsuarioId { get; private set; }
    public int EmpleadoId { get; private set; }
    public bool Activo { get; private set; } = true;

    public Usuario Usuario { get; private set; } = null!;
    public Empleado Empleado { get; private set; } = null!;

    private UsuarioEmpleado()
    {
    }

    public UsuarioEmpleado(Guid usuarioId, int empleadoId)
    {
        UsuarioId = usuarioId;
        EmpleadoId = empleadoId;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
