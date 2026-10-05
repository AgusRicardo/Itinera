namespace Itinera.Domain.Propuestas;

public class Destino
{
    public int Id { get; private set; }
    public int CiudadId { get; private set; }
    public bool Activo { get; private set; } = true;
    public Ciudad Ciudad { get; private set; }
    public List<Actividad> Actividades { get; private set; } = new();

    public Destino(int ciudadId, List<Actividad> actividades)
    {
        CiudadId = ciudadId;
        Actividades = actividades;
    }

    public void AgregarActividad(Actividad actividad)
    {
        Actividades.Add(actividad);
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
