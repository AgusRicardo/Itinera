using Itinera.Application.Actividades.Interfaces;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Mappers;
using Itinera.Application.Propuestas.Models;
using Itinera.Domain.Common;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Propuestas.Services;

public class PropuestaService(
    IPropuestaRepository propuestaRepository,
    IClienteRepository clienteRepository,
    IEmpleadoRepository empleadoRepository,
    IDestinoRepository destinoRepository,
    IActividadRepository actividadRepository,
    IGeneradorItinerarioIA generadorItinerario) : IPropuestaService
{
    private readonly IPropuestaRepository _propuestaRepository = propuestaRepository;
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IEmpleadoRepository _empleadoRepository = empleadoRepository;
    private readonly IDestinoRepository _destinoRepository = destinoRepository;
    private readonly IActividadRepository _actividadRepository = actividadRepository;
    private readonly IGeneradorItinerarioIA _generadorItinerario = generadorItinerario;

    public async Task<PropuestaResponse> CrearAsync(CrearPropuestaRequest request)
    {
        Cliente cliente = await ObtenerClienteById(request.ClienteId);
        Empleado empleado = await ObtenerEmpleadoById(request.EmpleadoId);
        List<Destino> destinos = await ObtenerDestinosByIds(request.DestinoIds);

        var propuesta = new Propuesta(cliente, empleado, request.Presupuesto);
        cliente.AsociarPropuesta(propuesta);
        empleado.AsociarPropuesta(propuesta);

        var generado = await _generadorItinerario.GenerarAsync(new SolicitudItinerario(
            request.FechaInicio,
            request.FechaFin,
            request.Presupuesto,
            request.Preferencias,
            destinos));

        AplicarItinerario(propuesta.Itinerario, generado, request.FechaInicio, request.FechaFin);

        await _propuestaRepository.AddAsync(propuesta);

        return PropuestaMapper.ToResponse(propuesta);
    }

    public async Task<PropuestaResponse> ObtenerPorIdAsync(int id)
    {
        var propuesta = await ObtenerPropuestaById(id);

        return PropuestaMapper.ToResponse(propuesta);
    }

    public async Task<List<PropuestaResponse>> ObtenerPorClienteAsync(int clienteId)
    {
        var propuestas = await _propuestaRepository.GetByClienteAsync(clienteId);

        return PropuestaMapper.ToResponse(propuestas);
    }

    public async Task<ItinerarioResponse> ObtenerItinerarioAsync(int propuestaId)
    {
        var propuesta = await ObtenerPropuestaById(propuestaId);

        return ItinerarioMapper.ToResponse(propuesta.Itinerario);
    }

    public async Task<PropuestaResponse> PresentarAsync(int id)
    {
        var propuesta = await ObtenerPropuestaById(id);

        propuesta.Presentar();

        await _propuestaRepository.UpdateAsync(propuesta);

        return PropuestaMapper.ToResponse(propuesta);
    }

    public async Task<PropuestaResponse> RegistrarRespuestaAsync(int id, RegistrarRespuestaRequest request)
    {
        var propuesta = await ObtenerPropuestaById(id);

        switch (request.Respuesta)
        {
            case RespuestaCliente.Aceptada:
                propuesta.Aceptar();
                break;
            case RespuestaCliente.Rechazada:
                propuesta.Rechazar();
                break;
            case RespuestaCliente.SolicitaModificaciones:
                propuesta.SolicitarModificaciones();
                break;
            default:
                throw new ArgumentException("Respuesta del cliente inválida.", nameof(request));
        }

        await _propuestaRepository.UpdateAsync(propuesta);

        return PropuestaMapper.ToResponse(propuesta);
    }

    public async Task<PropuestaResponse> ModificarItinerarioAsync(int id, ModificarItinerarioRequest request)
    {
        var propuesta = await ObtenerPropuestaById(id);

        if (propuesta.Estado != EstadoPropuesta.Borrador)
            throw new InvalidOperationException(
                "Solo se puede modificar el itinerario de una propuesta en estado Borrador.");

        var destinos = await ConstruirDestinosItinerario(request.Destinos);

        propuesta.Itinerario.DefinirFechas(request.FechaInicio, request.FechaFin);
        propuesta.Itinerario.ReemplazarDestinos(destinos);

        await _propuestaRepository.UpdateAsync(propuesta);

        return PropuestaMapper.ToResponse(propuesta);
    }

    private static void AplicarItinerario(
        Itinerario itinerario,
        ItinerarioGenerado generado,
        DateOnly fechaInicio,
        DateOnly fechaFin)
    {
        itinerario.DefinirNombre(generado.Nombre);
        itinerario.DefinirFechas(fechaInicio, fechaFin);
        itinerario.ReemplazarDestinos(generado.Destinos.Select(ConstruirDestinoItinerario));
    }

    private static DestinoItinerario ConstruirDestinoItinerario(DestinoPlanificado plan)
    {
        var destinoItinerario = new DestinoItinerario(
            plan.Orden,
            plan.FechaLlegada,
            plan.FechaPartida,
            plan.Destino,
            new List<ActividadDestinoItinerario>());

        foreach (var actividad in plan.Actividades)
        {
            destinoItinerario.AgregarActividad(new ActividadDestinoItinerario(
                actividad.FechaHoraInicio,
                actividad.CostoFinal,
                actividad.Observaciones,
                actividad.Orden,
                destinoItinerario,
                actividad.Actividad));
        }

        return destinoItinerario;
    }

    private async Task<List<DestinoItinerario>> ConstruirDestinosItinerario(
        IEnumerable<DestinoItinerarioRequest> destinos)
    {
        var resultado = new List<DestinoItinerario>();

        foreach (var destinoRequest in destinos)
        {
            Destino destino = await ObtenerDestinoById(destinoRequest.DestinoId);

            var destinoItinerario = new DestinoItinerario(
                destinoRequest.Orden,
                destinoRequest.FechaLlegada,
                destinoRequest.FechaPartida,
                destino,
                new List<ActividadDestinoItinerario>());

            foreach (var actividadRequest in destinoRequest.Actividades)
            {
                Actividad actividad = await ObtenerActividadById(actividadRequest.ActividadId);

                destinoItinerario.AgregarActividad(new ActividadDestinoItinerario(
                    actividadRequest.FechaHoraInicio,
                    actividadRequest.CostoFinal,
                    actividadRequest.Observaciones,
                    actividadRequest.Orden,
                    destinoItinerario,
                    actividad));
            }

            resultado.Add(destinoItinerario);
        }

        return resultado;
    }

    private async Task<Propuesta> ObtenerPropuestaById(int id)
    {
        var propuesta = await _propuestaRepository.GetByIdAsync(id);

        return propuesta ?? throw new PropuestaNoEncontradaException(id);
    }

    private async Task<Cliente> ObtenerClienteById(int clienteId)
    {
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);

        return cliente ?? throw new ClienteNoEncontradoException(clienteId);
    }

    private async Task<Empleado> ObtenerEmpleadoById(int empleadoId)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(empleadoId);

        return empleado ?? throw new EmpleadoNoEncontradoException(empleadoId);
    }

    private async Task<List<Destino>> ObtenerDestinosByIds(IEnumerable<int> destinoIds)
        => await _destinoRepository.ObtenerPorIdsConActividadesAsync(destinoIds);

    private async Task<Destino> ObtenerDestinoById(int destinoId)
    {
        var destino = await _destinoRepository.GetByIdAsync(destinoId);

        return destino ?? throw new DestinoNoEncontradoException(destinoId);
    }

    private async Task<Actividad> ObtenerActividadById(int actividadId)
    {
        var actividad = await _actividadRepository.GetByIdAsync(actividadId);

        return actividad ?? throw new ActividadNoEncontradaException(actividadId);
    }
}
