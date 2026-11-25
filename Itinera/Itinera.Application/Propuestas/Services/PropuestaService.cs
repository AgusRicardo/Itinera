using Itinera.Application.Common.Services;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Common.Interfaces;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Etiquetas.Interfaces;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Mappers;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Domain.Common;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Propuestas.Services;

public class PropuestaService(
    IPropuestaRepository propuestaRepository,
    IClienteRepository clienteRepository,
    IEmpleadoRepository empleadoRepository,
    IDestinoRepository destinoRepository,
    IEtiquetaRepository etiquetaRepository,
    ArmadorItinerario armador,
    PropuestaMapper mapper,
    IAuditoriaService auditoriaService)
    : PaginadoService<Propuesta, PropuestaResponse, ListadoComercialRequest>(propuestaRepository), IPropuestaService
{
    private readonly IPropuestaRepository _propuestaRepository = propuestaRepository;
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IEmpleadoRepository _empleadoRepository = empleadoRepository;
    private readonly IDestinoRepository _destinoRepository = destinoRepository;
    private readonly IEtiquetaRepository _etiquetaRepository = etiquetaRepository;
    private readonly ArmadorItinerario _armador = armador;
    private readonly PropuestaMapper _mapper = mapper;
    private readonly IAuditoriaService _auditoriaService = auditoriaService;

    public async Task<PropuestaResponse> CrearAsync(CrearPropuestaRequest request)
    {
        ValidarFechas(request.FechaInicio, request.FechaFin);

        Cliente cliente = await ObtenerClienteById(request.ClienteId);
        Empleado empleado = await ObtenerEmpleadoById(request.EmpleadoId);

        var propuesta = new Propuesta(
            cliente,
            empleado,
            request.Presupuesto,
            request.CantidadViajeros,
            request.Preferencias,
            request.IntensidadViaje,
            request.FechaInicio,
            request.FechaFin);
        await AplicarConfiguracion(propuesta, request);
        cliente.AsociarPropuesta(propuesta);
        empleado.AsociarPropuesta(propuesta);

        await _propuestaRepository.AddAsync(propuesta);
        await _auditoriaService.RegistrarAsync(
            AccionAuditoria.CrearPropuesta,
            nameof(Propuesta),
            propuesta.Id);

        return _mapper.ToResponse(propuesta);
    }

    public async Task<PropuestaResponse> ActualizarAsync(int id, ActualizarPropuestaRequest request)
    {
        ValidarFechas(request.FechaInicio, request.FechaFin);
        var propuesta = await ObtenerPropuestaById(id);

        propuesta.Actualizar(
            request.Presupuesto,
            request.CantidadViajeros,
            request.Preferencias,
            request.IntensidadViaje,
            request.FechaInicio,
            request.FechaFin);
        await AplicarConfiguracion(propuesta, request);

        return await Guardar(propuesta, AccionAuditoria.ModificarPropuesta);
    }

    public async Task<PropuestaResponse> GuardarItinerarioAsync(
        int id,
        int destinoId,
        GuardarItinerarioRequest request)
    {
        var propuesta = await ObtenerPropuestaById(id);
        var actividades = await _armador.ConstruirAsync(propuesta, destinoId, request.Actividades);

        propuesta.GuardarItinerario(destinoId, string.Empty, actividades);

        return await Guardar(propuesta, AccionAuditoria.ModificarItinerario);
    }

    private async Task AplicarConfiguracion(Propuesta propuesta, ConfiguracionPropuestaRequest request)
    {
        var destinos = await ObtenerDestinosByIds(request.Destinos.Select(item => item.DestinoId));
        var etiquetas = (await _etiquetaRepository.GetByIdsAsync(request.EtiquetaIds))
            .Concat(propuesta.Etiquetas
                .Select(item => item.Etiqueta)
                .Where(etiqueta => request.EtiquetaIds.Contains(etiqueta.Id)))
            .DistinctBy(etiqueta => etiqueta.Id)
            .ToList();

        if (etiquetas.Count != request.EtiquetaIds.Distinct().Count())
            throw new ArgumentException("Una o más etiquetas no están disponibles.", nameof(request));

        propuesta.DefinirDestinos(request.Destinos
            .Select(item => new Estadia(
                destinos.FirstOrDefault(destino => destino.Id == item.DestinoId)
                    ?? propuesta.ObtenerDestino(item.DestinoId)?.Destino
                    ?? throw new DestinoNoEncontradoException(item.DestinoId),
                item.FechaLlegada,
                item.FechaPartida))
            .ToList());

        var actividades = destinos
            .SelectMany(destino => destino.Actividades)
            .Concat(propuesta.Actividades.Select(item => item.Actividad))
            .Where(actividad => request.ActividadIds.Contains(actividad.Id))
            .DistinctBy(actividad => actividad.Id)
            .ToList();

        if (actividades.Count != request.ActividadIds.Distinct().Count())
            throw new ArgumentException(
                "Una o más actividades no pertenecen a los destinos seleccionados.",
                nameof(request));

        propuesta.DefinirActividades(actividades);
        propuesta.ReemplazarEtiquetas(etiquetas);
    }

    protected override List<PropuestaResponse> Convertir(IEnumerable<Propuesta> entidades)
        => _mapper.ToResponse(entidades);

    private async Task<PropuestaResponse> Guardar(Propuesta propuesta, AccionAuditoria accion)
    {
        await _propuestaRepository.UpdateAsync(propuesta);
        await _auditoriaService.RegistrarAsync(accion, nameof(Propuesta), propuesta.Id);

        return _mapper.ToResponse(propuesta);
    }

    public async Task<PropuestaResponse> ObtenerPorIdAsync(int id)
    {
        var propuesta = await ObtenerPropuestaById(id);

        return _mapper.ToResponse(propuesta);
    }

    public async Task<List<PropuestaResponse>> ObtenerPorClienteAsync(int clienteId)
    {
        var propuestas = await _propuestaRepository.GetByClienteAsync(clienteId);

        return _mapper.ToResponse(propuestas);
    }

    public Task<ResumenPropuestasResponse> ObtenerResumenAsync()
        => _propuestaRepository.GetResumenAsync();

    public async Task<List<PropuestaResponse>> ObtenerTodosAsync()
    {
        var propuestas = await _propuestaRepository.GetAllAsync();

        return _mapper.ToResponse(propuestas);
    }

    public async Task<PropuestaResponse> PresentarAsync(int id)
    {
        var propuesta = await ObtenerPropuestaById(id);

        propuesta.Presentar();

        await _propuestaRepository.UpdateAsync(propuesta);
        await _auditoriaService.RegistrarAsync(
            AccionAuditoria.PresentarPropuesta,
            nameof(Propuesta),
            propuesta.Id);

        return _mapper.ToResponse(propuesta);
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
        await _auditoriaService.RegistrarAsync(
            AccionAuditoria.RegistrarRespuestaPropuesta,
            nameof(Propuesta),
            propuesta.Id,
            request.Respuesta.ToString());

        return _mapper.ToResponse(propuesta);
    }

    public async Task EliminarAsync(int id)
    {
        var propuesta = await ObtenerPropuestaById(id);

        propuesta.Eliminar();

        await _propuestaRepository.UpdateAsync(propuesta);
        await _auditoriaService.RegistrarAsync(
            AccionAuditoria.EliminarPropuesta,
            nameof(Propuesta),
            propuesta.Id);
    }

    private static void ValidarFechas(DateOnly fechaInicio, DateOnly fechaFin)
    {
        if (fechaInicio == default)
            throw new ArgumentException("La fecha de inicio es obligatoria.", nameof(fechaInicio));

        if (fechaFin == default)
            throw new ArgumentException("La fecha de fin es obligatoria.", nameof(fechaFin));

        if (fechaFin < fechaInicio)
            throw new ArgumentException(
                "La fecha de fin no puede ser anterior a la fecha de inicio.", nameof(fechaFin));
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

    private Task<List<Destino>> ObtenerDestinosByIds(IEnumerable<int> destinoIds)
        => _destinoRepository.GetByIdsConActividadesAsync(destinoIds);
}
