namespace Itinera.Domain.Propuestas;

public class Pais
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public ICollection<Ciudad> Ciudades { get; private set; }
}
