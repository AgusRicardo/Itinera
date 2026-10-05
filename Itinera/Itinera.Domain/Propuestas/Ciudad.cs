namespace Itinera.Domain.Propuestas;

public class Ciudad
{
    public int Id { get; private set; }
    public int PaisId { get; private set; }
    public string Nombre { get; private set; }
    public bool Activo { get; private set; } = true;

    public Pais Pais { get; private set; }
    public ICollection<Destino> Destinos { get; private set; }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
