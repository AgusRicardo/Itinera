using Itinera.Application.Actividades.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Common.Interfaces;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Mappers;
using Itinera.Application.Propuestas.Models;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Domain.Common;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Propuestas.Services;

public sealed class GeneradorItinerariosService(
    IPropuestaRepository propuestaRepository,
    IGeneradorItinerarioIA generador,
    PerfilIntensidadResolver perfilResolver,
    IActividadRepository actividadRepository,
    IDestinoRepository destinoRepository,
    AjustadorItinerarioGenerado ajustador,
    ValidadorItinerarioGenerado validador,
    ArmadorItinerario armador,
    PropuestaMapper mapper,
    IAuditoriaService auditoriaService) : IGeneradorItinerariosService
{
    private const int LargoMaximoTexto = 500;

    public async Task<List<AlternativaItinerarioResponse>> GenerarAlternativasAsync(
        int propuestaId,
        int destinoId,
        GenerarAlternativasRequest request,
        CancellationToken cancellationToken = default)
    {
        var propuesta = await ObtenerPropuestaEditable(propuestaId);
        var estadia = ObtenerEstadia(propuesta, destinoId);

        return await Generar(
            propuesta,
            estadia,
            estadia.Destino,
            propuesta.ObtenerActividadesPedidas(destinoId),
            request.Preferencias,
            cancellationToken);
    }

    public async Task<PropuestaResponse> AplicarAlternativaAsync(
        int propuestaId,
        int destinoId,
        AplicarAlternativaRequest request,
        CancellationToken cancellationToken = default)
    {
        var propuesta = await ObtenerPropuestaEditable(propuestaId);
        var actividades = await armador.ConstruirAsync(
            propuesta,
            destinoId,
            request.Alternativa.Actividades.Select(ToRequest));

        ValidarPedidas(propuesta.ObtenerActividadesPedidas(destinoId), actividades);
        propuesta.GuardarItinerario(destinoId, Recortar(request.Alternativa.Nombre, 150), actividades);

        return await Guardar(propuesta, AccionAuditoria.AplicarItinerarioIA);
    }

    public async Task<List<AlternativaItinerarioResponse>> GenerarCambioDestinoAsync(
        int propuestaId,
        int destinoId,
        GenerarCambioDestinoRequest request,
        CancellationToken cancellationToken = default)
    {
        var propuesta = await ObtenerPropuestaEditable(propuestaId);
        var estadia = ObtenerEstadia(propuesta, destinoId);
        ValidarNuevoDestino(propuesta, request.NuevoDestinoId);

        return await Generar(
            propuesta,
            estadia,
            await ObtenerDestinoById(request.NuevoDestinoId),
            [],
            request.Preferencias,
            cancellationToken);
    }

    public async Task<PropuestaResponse> CambiarDestinoAsync(
        int propuestaId,
        int destinoId,
        CambiarDestinoRequest request,
        CancellationToken cancellationToken = default)
    {
        var propuesta = await ObtenerPropuestaEditable(propuestaId);
        ObtenerEstadia(propuesta, destinoId);
        ValidarNuevoDestino(propuesta, request.NuevoDestinoId);

        propuesta.CambiarDestino(destinoId, await ObtenerDestinoById(request.NuevoDestinoId));
        propuesta.GuardarItinerario(
            request.NuevoDestinoId,
            Recortar(request.Alternativa.Nombre, 150),
            await armador.ConstruirAsync(
                propuesta,
                request.NuevoDestinoId,
                request.Alternativa.Actividades.Select(ToRequest)));

        return await Guardar(propuesta, AccionAuditoria.CambiarDestinoPropuesta);
    }

    public async Task<List<AlternativaDiaResponse>> GenerarAlternativasDiaAsync(
        int propuestaId,
        int destinoId,
        ModificarDiaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Instrucciones))
            throw new ArgumentException("Las instrucciones de modificación son obligatorias.");

        var propuesta = await ObtenerPropuestaEditable(propuestaId);
        var estadia = ObtenerEstadia(propuesta, destinoId);
        var itinerario = ObtenerItinerario(propuesta, estadia, request.Fecha);
        var otrosDias = itinerario.Actividades.Where(item => item.Fecha != request.Fecha).ToList();
        var restricciones = CrearSolicitud(
            propuesta,
            estadia,
            estadia.Destino,
            ObtenerPedidasDelDia(propuesta, itinerario, request.Fecha),
            propuesta.Preferencias,
            propuesta.PresupuestoDisponiblePara(destinoId) - otrosDias.Sum(item => item.CostoFinal));
        var solicitud = new SolicitudModificacionDia(
            request.Fecha,
            request.Instrucciones,
            restricciones.Presupuesto,
            restricciones.CantidadViajeros,
            restricciones.Preferencias,
            restricciones.Etiquetas,
            restricciones.IntensidadViaje,
            restricciones.Perfil,
            CrearDias(estadia, itinerario),
            restricciones.Destinos);

        var alternativas = await generador.GenerarModificacionDiaAsync(solicitud, cancellationToken);
        var catalogo = await ObtenerCatalogo(estadia.Destino);
        var idsDeOtrosDias = otrosDias.Select(item => item.ActividadId).ToHashSet();

        var respuesta = new List<AlternativaDiaResponse>();
        foreach (var alternativa in alternativas.Where(item => item.Fecha == request.Fecha))
        {
            var generado = new ItinerarioGenerado(
                string.Empty,
                alternativa.Descripcion,
                0,
                [CrearDestinoPlanificado(estadia, estadia.Destino, alternativa.Actividades)]);
            var normalizado = ajustador.Ajustar(
                Normalizar(generado, restricciones, catalogo),
                restricciones);
            var actividades = normalizado.Destinos.SelectMany(destino => destino.Actividades).ToList();

            if (actividades.Any(actividad =>
                    DateOnly.FromDateTime(actividad.FechaHoraInicio) != request.Fecha
                    || (actividad.ActividadId is int id && idsDeOtrosDias.Contains(id)))
                || !validador.EsValido(normalizado, restricciones))
                continue;

            var guardado = ItinerarioGeneradoMapper.ToResponse(
                await GuardarRecomendaciones(normalizado, catalogo),
                restricciones);
            respuesta.Add(new AlternativaDiaResponse
            {
                Fecha = alternativa.Fecha,
                Descripcion = DescribirDia(guardado.Actividades),
                CostoEstimado = guardado.CostoEstimado,
                Actividades = guardado.Actividades
            });
        }

        return respuesta;
    }

    public async Task<PropuestaResponse> AplicarAlternativaDiaAsync(
        int propuestaId,
        int destinoId,
        AplicarModificacionDiaRequest request,
        CancellationToken cancellationToken = default)
    {
        var propuesta = await ObtenerPropuestaEditable(propuestaId);
        var fecha = request.Alternativa.Fecha;
        var itinerario = ObtenerItinerario(propuesta, ObtenerEstadia(propuesta, destinoId), fecha);
        var solicitadas = request.Alternativa.Actividades.Select(ToRequest).ToList();

        if (solicitadas.Any(item => DateOnly.FromDateTime(item.FechaHoraInicio) != fecha))
            throw new ArgumentException("La modificación solo puede incluir actividades del día indicado.");

        var actividades = await armador.ConstruirAsync(
            propuesta,
            destinoId,
            itinerario.Actividades
                .Where(item => item.Fecha != fecha)
                .Select(item => new ActividadItinerarioRequest
                {
                    ActividadId = item.ActividadId,
                    FechaHoraInicio = item.FechaHoraInicio,
                    Observaciones = item.Observaciones
                })
                .Concat(solicitadas));

        ValidarPedidas(ObtenerPedidasDelDia(propuesta, itinerario, fecha), actividades);
        propuesta.GuardarItinerario(destinoId, string.Empty, actividades);

        return await Guardar(propuesta, AccionAuditoria.AplicarItinerarioIA);
    }

    private async Task<List<AlternativaItinerarioResponse>> Generar(
        Propuesta propuesta,
        PropuestaDestino estadia,
        Destino destino,
        List<Actividad> pedidas,
        string? preferencias,
        CancellationToken cancellationToken)
    {
        var solicitud = CrearSolicitud(
            propuesta,
            estadia,
            destino,
            pedidas,
            preferencias ?? propuesta.Preferencias,
            propuesta.PresupuestoDisponiblePara(estadia.DestinoId));

        if (pedidas.Sum(actividad => actividad.CostoBase * propuesta.CantidadViajeros) > solicitud.Presupuesto)
            throw new InvalidOperationException(
                "Las actividades pedidas por el cliente superan el presupuesto disponible.");

        solicitud = solicitud with
        {
            PresupuestoObjetivo = propuesta.PresupuestoObjetivoPara(estadia.DestinoId)
        };
        var catalogo = await ObtenerCatalogo(destino);
        var validas = await GenerarValidas(estadia, destino, solicitud, catalogo, cancellationToken);

        if (!validas.Any(alternativa => ajustador.EsAceptable(alternativa, solicitud)))
            validas.AddRange(await GenerarValidas(estadia, destino, solicitud, catalogo, cancellationToken));

        var respuesta = new List<AlternativaItinerarioResponse>();
        foreach (var alternativa in ajustador.Elegir(validas, solicitud))
        {
            respuesta.Add(ItinerarioGeneradoMapper.ToResponse(
                await GuardarRecomendaciones(alternativa, catalogo),
                solicitud));
        }

        return respuesta;
    }

    private async Task<List<ItinerarioGenerado>> GenerarValidas(
        PropuestaDestino estadia,
        Destino destino,
        SolicitudItinerario solicitud,
        List<Destino> catalogo,
        CancellationToken cancellationToken)
    {
        var validas = new List<ItinerarioGenerado>();

        foreach (var alternativa in await generador.GenerarAsync(solicitud, cancellationToken))
        {
            var actividades = (alternativa.Destinos ?? [])
                .Where(item => item.DestinoId == destino.Id)
                .SelectMany(item => item.Actividades ?? [])
                .ToList();
            var normalizada = ajustador.Ajustar(
                Normalizar(
                    alternativa with { Destinos = [CrearDestinoPlanificado(estadia, destino, actividades)] },
                    solicitud,
                    catalogo),
                solicitud);

            if (normalizada.Destinos.Any(item => item.Actividades.Count > 0)
                && validador.EsValido(normalizada, solicitud))
                validas.Add(normalizada);
        }

        return validas;
    }

    private SolicitudItinerario CrearSolicitud(
        Propuesta propuesta,
        PropuestaDestino estadia,
        Destino destino,
        List<Actividad> pedidas,
        string preferencias,
        decimal presupuesto)
        => new(
            estadia.FechaLlegada,
            estadia.FechaPartida,
            presupuesto,
            propuesta.CantidadViajeros,
            preferencias,
            ObtenerNombresEtiquetas(propuesta),
            propuesta.IntensidadViaje,
            perfilResolver.Resolver(propuesta.IntensidadViaje),
            [CrearDestino(destino, pedidas)]);

    private async Task<Propuesta> ObtenerPropuestaEditable(int id)
    {
        var propuesta = await propuestaRepository.GetByIdAsync(id)
            ?? throw new PropuestaNoEncontradaException(id);

        propuesta.AsegurarEditable();

        return propuesta;
    }

    private static PropuestaDestino ObtenerEstadia(Propuesta propuesta, int destinoId)
        => propuesta.ObtenerDestino(destinoId)
            ?? throw new DestinoNoEncontradoException(destinoId);

    private static Itinerario ObtenerItinerario(Propuesta propuesta, PropuestaDestino estadia, DateOnly fecha)
    {
        if (!estadia.Incluye(fecha))
            throw new ArgumentException("La fecha indicada no pertenece al itinerario.");

        return propuesta.ObtenerItinerario(estadia.DestinoId)
            ?? throw new InvalidOperationException("El destino todavía no tiene un itinerario.");
    }

    private async Task<Destino> ObtenerDestinoById(int id)
        => await destinoRepository.GetByIdAsync(id)
            ?? throw new DestinoNoEncontradoException(id);

    private async Task<List<Destino>> ObtenerCatalogo(Destino destino)
    {
        var catalogo = await destinoRepository.GetByIdsConActividadesAsync([destino.Id]);

        return catalogo.Count > 0 ? catalogo : [destino];
    }

    private async Task<PropuestaResponse> Guardar(Propuesta propuesta, AccionAuditoria accion)
    {
        await propuestaRepository.UpdateAsync(propuesta);
        await auditoriaService.RegistrarAsync(accion, nameof(Propuesta), propuesta.Id);

        return mapper.ToResponse(propuesta);
    }

    private static void ValidarNuevoDestino(Propuesta propuesta, int nuevoDestinoId)
    {
        if (propuesta.ObtenerDestino(nuevoDestinoId) is not null)
            throw new ArgumentException("El nuevo destino ya forma parte de la propuesta.");
    }

    private static void ValidarPedidas(
        IEnumerable<Actividad> pedidas,
        List<ActividadItinerario> actividades)
    {
        if (pedidas.Any(pedida => actividades.All(item => item.ActividadId != pedida.Id)))
            throw new ArgumentException(
                "La alternativa debe incluir todas las actividades pedidas por el cliente.");
    }

    private static List<Actividad> ObtenerPedidasDelDia(
        Propuesta propuesta,
        Itinerario itinerario,
        DateOnly fecha)
        => propuesta.ObtenerActividadesPedidas(itinerario.DestinoId)
            .Where(pedida => itinerario.Actividades.Any(item =>
                item.ActividadId == pedida.Id && item.Fecha == fecha))
            .ToList();

    // La descripción de la IA puede nombrar actividades que el ajuste quitó; se arma con lo que queda.
    private static string DescribirDia(List<ActividadAlternativaResponse> actividades)
    {
        var nombres = actividades.Select(actividad => actividad.Nombre).ToList();

        return nombres.Count switch
        {
            0 => "Día libre.",
            1 => $"Incluye {nombres[0]}.",
            _ => $"Incluye {string.Join(", ", nombres[..^1])} y {nombres[^1]}."
        };
    }

    private static ActividadItinerarioRequest ToRequest(ActividadAlternativaResponse actividad)
        => new()
        {
            ActividadId = actividad.ActividadId,
            FechaHoraInicio = actividad.FechaHoraInicio,
            Observaciones = Recortar(actividad.Observaciones),
            RecomendadaPorIA = !actividad.PedidaPorCliente
        };

    private static List<string> ObtenerNombresEtiquetas(Propuesta propuesta)
        => propuesta.Etiquetas
            .Where(item => item.Etiqueta is not null)
            .Select(item => item.Etiqueta.Nombre)
            .ToList();

    private static DestinoSolicitado CrearDestino(Destino destino, IEnumerable<Actividad> pedidas)
        => new(
            destino.Id,
            destino.Ciudad?.Nombre ?? string.Empty,
            destino.Ciudad?.Pais?.Nombre ?? string.Empty,
            pedidas
                .Select(actividad => new ActividadSolicitada(
                    actividad.Id,
                    actividad.Nombre,
                    actividad.Descripcion,
                    actividad.CostoBase,
                    actividad.DuracionEstimada))
                .ToList());

    private static DestinoPlanificado CrearDestinoPlanificado(
        PropuestaDestino estadia,
        Destino destino,
        IReadOnlyList<ActividadPlanificada>? actividades)
        => new(
            1,
            estadia.FechaLlegada,
            estadia.FechaPartida,
            destino.Id,
            actividades ?? []);

    private static List<DiaItinerarioContexto> CrearDias(PropuestaDestino estadia, Itinerario itinerario)
        => itinerario.Actividades
            .Where(item => item.Actividad is not null)
            .GroupBy(item => item.Fecha)
            .Select(grupo => new DiaItinerarioContexto(
                grupo.Key,
                estadia.DestinoId,
                estadia.Destino.Ciudad?.Nombre ?? string.Empty,
                grupo.Select(item => new ActividadPlanificada(
                    item.ActividadId,
                    item.Actividad.Nombre,
                    item.Actividad.Descripcion,
                    item.Actividad.CostoBase,
                    item.Actividad.DuracionEstimada,
                    item.FechaHoraInicio,
                    item.CostoFinal,
                    item.Observaciones,
                    item.Orden,
                    estadia.DestinoId)).ToList()))
            .ToList();

    private static ItinerarioGenerado Normalizar(
        ItinerarioGenerado itinerario,
        SolicitudItinerario solicitud,
        List<Destino> catalogo)
    {
        var destinos = (itinerario.Destinos ?? [])
            .Select(destino => destino with
            {
                Actividades = (destino.Actividades ?? [])
                    .Select(actividad => NormalizarActividad(
                        actividad,
                        destino.DestinoId,
                        solicitud,
                        catalogo))
                    .ToList()
            })
            .ToList();

        return itinerario with
        {
            Destinos = destinos,
            CostoEstimado = destinos
                .SelectMany(destino => destino.Actividades)
                .Sum(actividad => actividad.CostoFinal)
        };
    }

    private static ActividadPlanificada NormalizarActividad(
        ActividadPlanificada actividad,
        int destinoId,
        SolicitudItinerario solicitud,
        List<Destino> catalogo)
    {
        var nombre = actividad.Nombre?.Trim() ?? string.Empty;
        var obligatoria = solicitud.Destinos
            .Where(destino => destino.DestinoId == destinoId)
            .SelectMany(destino => destino.Actividades)
            .FirstOrDefault(item => item.ActividadId == actividad.ActividadId);
        var delCatalogo = obligatoria is null
            ? BuscarPorNombre(catalogo, destinoId, nombre)
            : null;
        var costoBase = obligatoria?.CostoBase ?? delCatalogo?.CostoBase ?? actividad.CostoBase;

        return actividad with
        {
            ActividadId = obligatoria?.ActividadId ?? delCatalogo?.Id,
            Nombre = obligatoria?.Nombre ?? delCatalogo?.Nombre ?? nombre,
            Descripcion = Recortar(
                obligatoria?.Descripcion ?? delCatalogo?.Descripcion ?? actividad.Descripcion),
            CostoBase = costoBase,
            DuracionEstimada = obligatoria?.DuracionEstimada
                ?? delCatalogo?.DuracionEstimada
                ?? actividad.DuracionEstimada,
            CostoFinal = costoBase * solicitud.CantidadViajeros,
            Observaciones = Recortar(actividad.Observaciones),
            DestinoId = destinoId
        };
    }

    private static Actividad? BuscarPorNombre(List<Destino> catalogo, int destinoId, string nombre)
        => catalogo
            .FirstOrDefault(destino => destino.Id == destinoId)?
            .Actividades
            .FirstOrDefault(actividad => actividad.Activo
                && string.Equals(actividad.Nombre.Trim(), nombre, StringComparison.OrdinalIgnoreCase));

    private static string Recortar(string? texto, int largoMaximo = LargoMaximoTexto)
        => texto is null
            ? string.Empty
            : texto.Length <= largoMaximo ? texto : texto[..largoMaximo];

    private async Task<ItinerarioGenerado> GuardarRecomendaciones(
        ItinerarioGenerado itinerario,
        List<Destino> catalogo)
    {
        var destinos = new List<DestinoPlanificado>();
        foreach (var destino in itinerario.Destinos)
        {
            var actividades = new List<ActividadPlanificada>();
            foreach (var actividad in destino.Actividades)
            {
                actividades.Add(actividad.ActividadId is null
                    ? actividad with { ActividadId = await GuardarRecomendacion(actividad, destino.DestinoId, catalogo) }
                    : actividad);
            }

            destinos.Add(destino with { Actividades = actividades });
        }

        return itinerario with { Destinos = destinos };
    }

    private async Task<int> GuardarRecomendacion(
        ActividadPlanificada recomendada,
        int destinoId,
        List<Destino> catalogo)
    {
        var existente = BuscarPorNombre(catalogo, destinoId, recomendada.Nombre);
        if (existente is not null)
            return existente.Id;

        var destino = catalogo.FirstOrDefault(item => item.Id == destinoId)
            ?? throw new DestinoNoEncontradoException(destinoId);
        var actividad = new Actividad(
            recomendada.Nombre,
            recomendada.Descripcion,
            recomendada.CostoBase,
            recomendada.DuracionEstimada,
            destino,
            generadoPorIA: true);
        await actividadRepository.AddAsync(actividad);

        if (!destino.Actividades.Contains(actividad))
            destino.AgregarActividad(actividad);

        return actividad.Id;
    }
}
