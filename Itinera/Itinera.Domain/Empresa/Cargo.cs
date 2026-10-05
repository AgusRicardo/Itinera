namespace Itinera.Domain.Empresa;

public class Cargo
{
    public int Id { get; private set; }
    public string Descripcion { get; private set; }
    public bool Activo { get; private set; } = true;

    public Cargo(string descripcion)
    {
        Descripcion = descripcion;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
