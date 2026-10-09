namespace Itinera.Application.Propuestas.Dtos;

public class RegistrarRespuestaRequest
{
    public RespuestaCliente Respuesta { get; set; }
}

public enum RespuestaCliente
{
    Aceptada = 1,
    Rechazada = 2,
    SolicitaModificaciones = 3
}
