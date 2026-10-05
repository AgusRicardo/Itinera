namespace Itinera.Domain.Propuestas;

public class Actividad
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public decimal CostoBase { get; private set; }
    public int DuracionEstimada { get; private set; }
    public int DestinoId { get; private set; }
    public bool Activo { get; private set; } = true;
    public Destino Destino { get; private set; }

    public Actividad(string nombre, string descripcion, decimal costo, int duracion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        CostoBase = costo;
        DuracionEstimada = duracion;
    }

    public void ActualizarInformacion(string nombre, string descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
