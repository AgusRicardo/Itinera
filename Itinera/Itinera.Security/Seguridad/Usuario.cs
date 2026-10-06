namespace Itinera.Security.Domain;

public class Usuario : UsuarioComponent
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool Activo { get; private set; }

    public Usuario(string nombre, string email, string passwordHash)
        : base(nombre)
    {
        Email = email;
        PasswordHash = passwordHash;
        Activo = true;
    }

    private Usuario() { }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
    public void ActualizarPassword(string nuevoHash) => PasswordHash = nuevoHash;
}
