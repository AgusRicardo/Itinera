using Itinera.Domain.Common;
using Itinera.Domain.Facturacion;
using Itinera.Domain.Usuarios;

namespace Itinera.Domain.Propuestas;

public class Propuesta
{
    public int Id { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public decimal Presupuesto { get; private set; }
    public int EstadoPropuestaId { get; private set; }
    public EstadoPropuesta Estado => (EstadoPropuesta)EstadoPropuestaId;
    public int ClienteId { get; private set; }
    public int EmpleadoId { get; private set; }
    public DateTime FechaRegistracion { get; protected set; }
    public Guid UsuarioRegistracionId { get; protected set; }
    public DateTime? FechaModificacion { get; protected set; }
    public Guid? UsuarioModificacionId { get; protected set; }

    public Cliente Cliente { get; private set; }
    public Empleado Empleado { get; private set; }
    public Itinerario Itinerario { get; private set; }
    public EstadoPropuestaCatalogo EstadoPropuestaCatalogo { get; private set; }
    public List<Factura> Facturas { get; private set; } = new();

    private Propuesta()
    {
    }

    public Propuesta(Cliente cliente, Empleado empleado, decimal presupuesto)
    {
        FechaCreacion = DateTime.UtcNow;
        EstadoPropuestaId = (int)EstadoPropuesta.Borrador;

        Cliente = cliente;
        Empleado = empleado;
        Presupuesto = presupuesto;

        Itinerario = new Itinerario();
    }

    public void GenerarItinerario(Itinerario itinerario)
    {
        Itinerario = itinerario;
    }

    public void AgregarDestino(DestinoItinerario destinoItinerario)
    {
        Itinerario.AgregarDestino(destinoItinerario);
    }

    public void Presentar() => CambiarEstado(EstadoPropuesta.Presentada);
    public void Aceptar() => CambiarEstado(EstadoPropuesta.Aceptada);
    public void Rechazar() => CambiarEstado(EstadoPropuesta.Rechazada);
    public void Eliminar() => CambiarEstado(EstadoPropuesta.Eliminada);

    private void CambiarEstado(EstadoPropuesta estado)
    {
        EstadoPropuestaId = (int)estado;
    }
}
