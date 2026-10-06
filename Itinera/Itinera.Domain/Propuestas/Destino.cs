namespace Itinera.Domain.Propuestas;

public class Destino
{
    public int Id { get; private set; }
    public int CiudadId { get; private set; }
    public bool Activo { get; private set; } = true;
    public Ciudad Ciudad { get; private set; }
    public List<Actividad> Actividades { get; private set; } = new();

    private Destino()
    {
    }

    public Destino(Ciudad ciudad)
    {
        ArgumentNullException.ThrowIfNull(ciudad);

        Ciudad = ciudad;
        CiudadId = ciudad.Id;
    }

    public void ActualizarDatos(Ciudad ciudad)
    {
        ArgumentNullException.ThrowIfNull(ciudad);

        Ciudad = ciudad;
        CiudadId = ciudad.Id;
    }

    public void AgregarActividad(Actividad actividad)
    {
        Actividades.Add(actividad);
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
