namespace Itinera.Domain.Empresa;

public class Cargo
{
    public int Id { get; private set; }
    public string Descripcion { get; private set; }

    public Cargo(string descripcion)
    {
        Descripcion = descripcion;
    }
}