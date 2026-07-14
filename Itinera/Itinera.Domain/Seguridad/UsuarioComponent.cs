namespace Itinera.Domain.Seguridad;

public abstract class UsuarioComponent
{
    public Guid Id { get; protected set; }
    public string Nombre { get; protected set; }

    protected UsuarioComponent(string nombre)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
    }

    protected UsuarioComponent() { }

    public virtual void Agregar(UsuarioComponent componente)
        => throw new InvalidOperationException("Solo los grupos pueden contener miembros.");

    public virtual void Quitar(UsuarioComponent componente)
        => throw new InvalidOperationException("Solo los grupos pueden contener miembros.");

    public virtual IReadOnlyList<UsuarioComponent> ObtenerMiembros()
        => throw new InvalidOperationException("Solo los grupos pueden contener miembros.");
}
