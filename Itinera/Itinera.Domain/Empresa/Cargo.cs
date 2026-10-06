namespace Itinera.Domain.Empresa;

public class Cargo
{
    public int Id { get; private set; }
    public string Descripcion { get; private set; }
    public bool Activo { get; private set; } = true;

    private Cargo()
    {
    }

    public Cargo(string descripcion)
    {
        ValidarDescripcion(descripcion);
        Descripcion = descripcion;
    }

    public void ActualizarDescripcion(string descripcion)
    {
        ValidarDescripcion(descripcion);
        Descripcion = descripcion;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    private static void ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción del cargo es obligatoria.", nameof(descripcion));
    }
}
