using Itinera.Application.Common.Exceptions;
using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Common.Services;
using Itinera.Application.Conceptos.Dtos;
using Itinera.Application.Conceptos.Interfaces;
using Itinera.Application.Conceptos.Mappers;
using Itinera.Domain.Facturacion;

namespace Itinera.Application.Conceptos.Services;

public sealed class ConceptoFacturaService(
    IConceptoFacturaRepository conceptoRepository,
    ITenantContext tenantContext)
    : PaginadoService<ConceptoFactura, ConceptoFacturaResponse, ListadoRequest>(conceptoRepository),
        IConceptoFacturaService
{
    private readonly IConceptoFacturaRepository _conceptoRepository = conceptoRepository;
    private readonly ITenantContext _tenantContext = tenantContext;

    public async Task<ConceptoFacturaResponse> ObtenerPorIdAsync(int id)
        => ConceptoFacturaMapper.ToResponse(await ObtenerConceptoById(id));

    public async Task<List<ConceptoFacturaResponse>> ObtenerTodosAsync()
        => ConceptoFacturaMapper.ToResponse(await _conceptoRepository.GetAllAsync());

    public async Task<ConceptoFacturaResponse> CrearAsync(CrearConceptoFacturaRequest request)
    {
        var empresaId = _tenantContext.EmpresaId
            ?? throw new InvalidOperationException("No se pudo determinar la empresa del usuario.");
        await ValidarDescripcionAsync(request.Descripcion, null);

        var concepto = new ConceptoFactura(request.Descripcion, request.Precio, empresaId, request.Porcentaje);
        await _conceptoRepository.AddAsync(concepto);

        return ConceptoFacturaMapper.ToResponse(concepto);
    }

    public async Task<ConceptoFacturaResponse> ActualizarAsync(int id, ActualizarConceptoFacturaRequest request)
    {
        var concepto = await ObtenerConceptoById(id);
        await ValidarDescripcionAsync(request.Descripcion, id);

        concepto.Actualizar(request.Descripcion, request.Precio, request.Porcentaje);
        await _conceptoRepository.UpdateAsync(concepto);

        return ConceptoFacturaMapper.ToResponse(concepto);
    }

    public async Task EliminarAsync(int id)
    {
        var concepto = await ObtenerConceptoById(id);
        concepto.Desactivar();
        await _conceptoRepository.UpdateAsync(concepto);
    }

    protected override List<ConceptoFacturaResponse> Convertir(IEnumerable<ConceptoFactura> entidades)
        => ConceptoFacturaMapper.ToResponse(entidades);

    private async Task ValidarDescripcionAsync(string descripcion, int? idExcluido)
    {
        if (await _conceptoRepository.ExisteAsync(descripcion.Trim(), idExcluido))
            throw new InvalidOperationException("Ya existe un concepto con esa descripción.");
    }

    private async Task<ConceptoFactura> ObtenerConceptoById(int id)
        => await _conceptoRepository.GetByIdAsync(id)
            ?? throw new ConceptoFacturaNoEncontradoException(id);
}
