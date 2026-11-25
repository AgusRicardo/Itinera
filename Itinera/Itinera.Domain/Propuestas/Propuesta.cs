using Itinera.Domain.Common;
using Itinera.Domain.Catalogos;
using Itinera.Domain.Facturacion;
using Itinera.Domain.Usuarios;

namespace Itinera.Domain.Propuestas;

public class Propuesta
{
    public int Id { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public decimal Presupuesto { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaFin { get; private set; }
    public int CantidadViajeros { get; private set; }
    public string Preferencias { get; private set; }
    public int IntensidadViajeId { get; private set; }
    public IntensidadViaje IntensidadViaje => (IntensidadViaje)IntensidadViajeId;
    public int EmpresaId { get; private set; }
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
    public List<Itinerario> Itinerarios { get; private set; } = new();
    public List<PropuestaDestino> Destinos { get; private set; } = new();
    public List<PropuestaActividad> Actividades { get; private set; } = new();
    public EstadoPropuestaCatalogo EstadoPropuestaCatalogo { get; private set; }
    public List<Factura> Facturas { get; private set; } = new();
    public List<PropuestaEtiqueta> Etiquetas { get; private set; } = new();

    private Propuesta()
    {
    }

    public Propuesta(
        Cliente cliente,
        Empleado empleado,
        decimal presupuesto,
        int cantidadViajeros = 1,
        string preferencias = "",
        IntensidadViaje intensidadViaje = IntensidadViaje.Media,
        DateOnly? fechaInicio = null,
        DateOnly? fechaFin = null)
    {
        if (cantidadViajeros < 1)
            throw new ArgumentOutOfRangeException(nameof(cantidadViajeros));

        FechaCreacion = DateTime.UtcNow;
        EstadoPropuestaId = (int)EstadoPropuesta.Borrador;

        Cliente = cliente;
        Empleado = empleado;
        Presupuesto = presupuesto;
        CantidadViajeros = cantidadViajeros;
        Preferencias = preferencias ?? string.Empty;
        IntensidadViajeId = (int)intensidadViaje;
        FechaInicio = fechaInicio ?? DateOnly.FromDateTime(DateTime.UtcNow);
        FechaFin = fechaFin ?? FechaInicio;
        EmpresaId = empleado.EmpresaId;
    }

    public void Actualizar(
        decimal presupuesto,
        int cantidadViajeros,
        string preferencias,
        IntensidadViaje intensidadViaje,
        DateOnly fechaInicio,
        DateOnly fechaFin)
    {
        AsegurarEditable();

        if (cantidadViajeros < 1)
            throw new ArgumentOutOfRangeException(nameof(cantidadViajeros));

        Presupuesto = presupuesto;
        CantidadViajeros = cantidadViajeros;
        Preferencias = preferencias ?? string.Empty;
        IntensidadViajeId = (int)intensidadViaje;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;

        foreach (var actividad in Itinerarios.SelectMany(itinerario => itinerario.Actividades))
            actividad.RecalcularCosto(cantidadViajeros);
    }

    public void DefinirDestinos(IEnumerable<Estadia> estadias)
    {
        AsegurarEditable();
        var ordenadas = estadias.OrderBy(estadia => estadia.FechaLlegada)
            .ThenBy(estadia => estadia.FechaPartida)
            .ToList();
        ValidarEstadias(ordenadas);

        foreach (var quitado in Destinos
                     .Where(item => ordenadas.All(estadia => estadia.Destino.Id != item.DestinoId))
                     .ToList())
            QuitarDestino(quitado);

        for (var indice = 0; indice < ordenadas.Count; indice++)
        {
            var estadia = ordenadas[indice];
            var existente = Destinos.FirstOrDefault(item => item.DestinoId == estadia.Destino.Id);

            if (existente is null)
                Destinos.Add(new PropuestaDestino(this, estadia, indice + 1));
            else
                existente.DefinirEstadia(estadia.FechaLlegada, estadia.FechaPartida, indice + 1);
        }
    }

    public void DefinirActividades(IEnumerable<Actividad> actividades)
    {
        AsegurarEditable();
        var nuevas = actividades.DistinctBy(actividad => actividad.Id).ToList();

        if (nuevas.Any(actividad => Destinos.All(item => item.DestinoId != actividad.DestinoId)))
            throw new ArgumentException(
                "Una o más actividades no pertenecen a los destinos de la propuesta.", nameof(actividades));

        Actividades.RemoveAll(item => nuevas.All(actividad => actividad.Id != item.ActividadId));

        foreach (var actividad in nuevas.Where(actividad =>
                     Actividades.All(item => item.ActividadId != actividad.Id)))
            Actividades.Add(new PropuestaActividad(this, actividad));
    }

    public void CambiarDestino(int destinoId, Destino nuevo)
    {
        AsegurarEditable();
        var actual = ObtenerDestino(destinoId)
            ?? throw new ArgumentException("El destino no pertenece a la propuesta.", nameof(destinoId));

        if (ObtenerDestino(nuevo.Id) is not null)
            throw new ArgumentException("El nuevo destino ya forma parte de la propuesta.", nameof(nuevo));

        QuitarDestino(actual);
        Destinos.Add(new PropuestaDestino(
            this,
            new Estadia(nuevo, actual.FechaLlegada, actual.FechaPartida),
            actual.Orden));
    }

    public Itinerario GuardarItinerario(
        int destinoId,
        string nombre,
        IEnumerable<ActividadItinerario> actividades)
    {
        AsegurarEditable();
        var destino = ObtenerDestino(destinoId)
            ?? throw new ArgumentException("El destino no pertenece a la propuesta.", nameof(destinoId));
        var itinerario = ObtenerItinerario(destinoId) ?? new Itinerario(destino.Destino);

        itinerario.ReemplazarActividades(actividades);

        if (!string.IsNullOrWhiteSpace(nombre))
            itinerario.DefinirNombre(nombre.Trim());
        else if (string.IsNullOrWhiteSpace(itinerario.Nombre))
            itinerario.DefinirNombre($"Itinerario de {destino.Destino.Ciudad?.Nombre}".Trim());

        if (!Itinerarios.Contains(itinerario))
            Itinerarios.Add(itinerario);

        return itinerario;
    }

    public PropuestaDestino ObtenerDestino(int destinoId)
        => Destinos.FirstOrDefault(item => item.DestinoId == destinoId);

    public Itinerario ObtenerItinerario(int destinoId)
        => Itinerarios.FirstOrDefault(item => item.DestinoId == destinoId);

    public bool UsaActividad(int actividadId)
        => Actividades.Any(item => item.ActividadId == actividadId)
            || Itinerarios.Any(itinerario =>
                itinerario.Actividades.Any(item => item.ActividadId == actividadId));

    public decimal CostoItinerarios => Itinerarios.Sum(itinerario => itinerario.Costo);

    public decimal PresupuestoDisponiblePara(int destinoId)
        => Presupuesto - Itinerarios
            .Where(itinerario => itinerario.DestinoId != destinoId)
            .Sum(itinerario => itinerario.Costo);

    public decimal PresupuestoObjetivoPara(int destinoId)
    {
        var porArmar = Destinos
            .Where(item => item.DestinoId == destinoId || ObtenerItinerario(item.DestinoId) is null)
            .ToList();
        var dias = porArmar.Sum(item => item.Dias);
        var diasDelDestino = porArmar.Where(item => item.DestinoId == destinoId).Sum(item => item.Dias);

        return dias == 0 ? 0 : PresupuestoDisponiblePara(destinoId) * diasDelDestino / dias;
    }

    public List<Actividad> ObtenerActividadesPedidas(int destinoId)
        => Actividades
            .Where(item => item.Actividad is not null && item.Actividad.DestinoId == destinoId)
            .Select(item => item.Actividad)
            .ToList();

    public List<string> ObtenerFaltantes(PropuestaDestino destino)
    {
        var itinerario = ObtenerItinerario(destino.DestinoId);
        if (itinerario is null || itinerario.Actividades.Count == 0)
            return new List<string> { "Falta armar el itinerario." };

        var faltantes = ObtenerActividadesPedidas(destino.DestinoId)
            .Where(actividad => !itinerario.IncluyeActividad(actividad.Id))
            .Select(actividad => $"Falta una actividad pedida por el cliente: {actividad.Nombre}.")
            .ToList();

        if (itinerario.Actividades.Any(actividad => !destino.Incluye(actividad.Fecha)))
            faltantes.Add("Hay actividades fuera de las fechas del destino.");

        return faltantes;
    }

    public bool TieneItinerariosCompletos()
        => Destinos.Count > 0 && Destinos.All(destino => ObtenerFaltantes(destino).Count == 0);

    public void AsegurarEditable()
    {
        if (Estado is EstadoPropuesta.Aceptada or EstadoPropuesta.Eliminada)
            throw new InvalidOperationException(
                $"No se puede modificar una propuesta en estado '{Estado}'.");
    }

    private void QuitarDestino(PropuestaDestino destino)
    {
        Destinos.Remove(destino);
        Itinerarios.RemoveAll(itinerario => itinerario.DestinoId == destino.DestinoId);
        Actividades.RemoveAll(item => item.Actividad?.DestinoId == destino.DestinoId);
    }

    private void ValidarEstadias(List<Estadia> estadias)
    {
        if (estadias.Count == 0)
            throw new ArgumentException("La propuesta debe tener al menos un destino.", nameof(estadias));

        if (estadias.DistinctBy(estadia => estadia.Destino.Id).Count() != estadias.Count)
            throw new ArgumentException("No se puede repetir un destino.", nameof(estadias));

        if (estadias.Any(estadia => estadia.FechaPartida < estadia.FechaLlegada
                || estadia.FechaLlegada < FechaInicio
                || estadia.FechaPartida > FechaFin))
            throw new ArgumentException(
                "Las fechas de cada destino deben estar dentro de las fechas del viaje.", nameof(estadias));

        for (var indice = 1; indice < estadias.Count; indice++)
        {
            if (estadias[indice].FechaLlegada < estadias[indice - 1].FechaPartida)
                throw new ArgumentException(
                    "Las fechas de los destinos no pueden superponerse.", nameof(estadias));
        }
    }

    public void AsignarEtiqueta(Etiqueta etiqueta)
    {
        if (Etiquetas.All(item => item.EtiquetaId != etiqueta.Id))
            Etiquetas.Add(new PropuestaEtiqueta(this, etiqueta));
    }

    public void ReemplazarEtiquetas(IEnumerable<Etiqueta> etiquetas)
    {
        var nuevas = etiquetas.ToList();
        Etiquetas.RemoveAll(item => nuevas.All(etiqueta => etiqueta.Id != item.EtiquetaId));
        foreach (var etiqueta in nuevas)
            AsignarEtiqueta(etiqueta);
    }

    public void Presentar()
    {
        AsegurarEstado(EstadoPropuesta.Borrador);

        AsegurarItinerariosCompletos();
        CambiarEstado(EstadoPropuesta.Presentada);
    }

    public void Aceptar()
    {
        AsegurarEstado(EstadoPropuesta.Presentada);
        AsegurarItinerariosCompletos();
        CambiarEstado(EstadoPropuesta.Aceptada);
    }

    public void Rechazar()
    {
        AsegurarEstado(EstadoPropuesta.Presentada);
        CambiarEstado(EstadoPropuesta.Rechazada);
    }

    public void SolicitarModificaciones()
    {
        AsegurarEstado(EstadoPropuesta.Presentada);
        CambiarEstado(EstadoPropuesta.Borrador);
    }

    public void Eliminar() => CambiarEstado(EstadoPropuesta.Eliminada);

    private void AsegurarItinerariosCompletos()
    {
        if (!TieneItinerariosCompletos())
            throw new InvalidOperationException(
                "Todos los destinos de la propuesta deben tener su itinerario completo.");
    }

    private void AsegurarEstado(EstadoPropuesta esperado)
    {
        if (Estado != esperado)
            throw new InvalidOperationException(
                $"La propuesta debe estar en estado '{esperado}' (estado actual: '{Estado}').");
    }

    private void CambiarEstado(EstadoPropuesta estado)
    {
        EstadoPropuestaId = (int)estado;
    }
}
