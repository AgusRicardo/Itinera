namespace Itinera.Security.Domain;

public class UsuarioComponentRol
{
    public Guid UsuarioComponentId { get; private set; }
    public int RolId { get; private set; }
    public UsuarioComponent UsuarioComponent { get; private set; }
    public Rol Rol { get; private set; }

    public UsuarioComponentRol(UsuarioComponent usuarioComponent, Rol rol)
    {
        UsuarioComponentId = usuarioComponent.Id;
        RolId = rol.Id;
        UsuarioComponent = usuarioComponent;
        Rol = rol;
    }

    private UsuarioComponentRol() { }
}
