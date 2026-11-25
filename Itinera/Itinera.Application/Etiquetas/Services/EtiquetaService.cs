using Itinera.Application.Common.Exceptions;
using Itinera.Application.Common.Interfaces;
using Itinera.Application.Etiquetas.Dtos;
using Itinera.Application.Etiquetas.Interfaces;
using Itinera.Application.Etiquetas.Mappers;
using Itinera.Domain.Catalogos;

namespace Itinera.Application.Etiquetas.Services;

public sealed class EtiquetaService(
    IEtiquetaRepository etiquetaRepository,
    ITenantContext tenantContext) : IEtiquetaService
{
    private readonly IEtiquetaRepository _etiquetaRepository = etiquetaRepository;
    private readonly ITenantContext _tenantContext = tenantContext;

    public async Task<EtiquetaResponse> CrearAsync(CrearEtiquetaRequest request)
    {
        var empresaId = ObtenerEmpresaId();
        await ValidarDuplicadoAsync(request.Nombre, empresaId, null);

        var etiqueta = new Etiqueta(request.Nombre, empresaId);
        await _etiquetaRepository.AddAsync(etiqueta);
        return EtiquetaMapper.ToResponse(etiqueta);
    }

    public async Task<EtiquetaResponse> ObtenerPorIdAsync(int id)
        => EtiquetaMapper.ToResponse(await ObtenerEtiquetaAsync(id));

    public async Task<List<EtiquetaResponse>> ObtenerTodosAsync()
        => EtiquetaMapper.ToResponse(await _etiquetaRepository.GetAllAsync());

    public async Task<EtiquetaResponse> ActualizarAsync(
        int id,
        ActualizarEtiquetaRequest request)
    {
        var etiqueta = await ObtenerEtiquetaAsync(id);
        await ValidarDuplicadoAsync(request.Nombre, etiqueta.EmpresaId, id);

        etiqueta.Actualizar(request.Nombre);
        await _etiquetaRepository.UpdateAsync(etiqueta);
        return EtiquetaMapper.ToResponse(etiqueta);
    }

    public async Task EliminarAsync(int id)
    {
        var etiqueta = await ObtenerEtiquetaAsync(id);
        etiqueta.Desactivar();
        await _etiquetaRepository.UpdateAsync(etiqueta);
    }

    private async Task ValidarDuplicadoAsync(string nombre, int empresaId, int? idExcluido)
    {
        if (await _etiquetaRepository.ExisteAsync(nombre.Trim(), empresaId, idExcluido))
            throw new InvalidOperationException("Ya existe una etiqueta con ese nombre.");
    }

    private async Task<Etiqueta> ObtenerEtiquetaAsync(int id)
    {
        var etiqueta = await _etiquetaRepository.GetByIdAsync(id);
        return etiqueta ?? throw new EtiquetaNoEncontradaException(id);
    }

    private int ObtenerEmpresaId()
        => _tenantContext.EmpresaId
           ?? throw new InvalidOperationException("No se pudo determinar la empresa del usuario.");
}
