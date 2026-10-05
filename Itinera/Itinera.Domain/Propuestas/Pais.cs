namespace Itinera.Domain.Propuestas;

public class Pais
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public bool Activo { get; private set; } = true;
    public ICollection<Ciudad> Ciudades { get; private set; }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
