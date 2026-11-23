using Itinera.Application.Propuestas.Dtos;

namespace Itinera.Application.Propuestas.Interfaces;

public interface IGeneradorItinerariosService
{
    Task<List<AlternativaItinerarioResponse>> GenerarAlternativasAsync(
        int propuestaId,
        int destinoId,
        GenerarAlternativasRequest request,
        CancellationToken cancellationToken = default);

    Task<PropuestaResponse> AplicarAlternativaAsync(
        int propuestaId,
        int destinoId,
        AplicarAlternativaRequest request,
        CancellationToken cancellationToken = default);

    Task<List<AlternativaDiaResponse>> GenerarAlternativasDiaAsync(
        int propuestaId,
        int destinoId,
        ModificarDiaRequest request,
        CancellationToken cancellationToken = default);

    Task<PropuestaResponse> AplicarAlternativaDiaAsync(
        int propuestaId,
        int destinoId,
        AplicarModificacionDiaRequest request,
        CancellationToken cancellationToken = default);

    Task<List<AlternativaItinerarioResponse>> GenerarCambioDestinoAsync(
        int propuestaId,
        int destinoId,
        GenerarCambioDestinoRequest request,
        CancellationToken cancellationToken = default);

    Task<PropuestaResponse> CambiarDestinoAsync(
        int propuestaId,
        int destinoId,
        CambiarDestinoRequest request,
        CancellationToken cancellationToken = default);
}
