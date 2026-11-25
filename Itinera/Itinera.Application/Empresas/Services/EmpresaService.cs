using Itinera.Application.Common.Exceptions;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Common.Services;
using Itinera.Application.Empresas.Dtos;
using Itinera.Application.Empresas.Interfaces;
using Itinera.Application.Empresas.Mappers;
using Itinera.Domain.Empresa;

namespace Itinera.Application.Empresas.Services;

public class EmpresaService(IEmpresaRepository empresaRepository)
    : PaginadoService<Empresa, EmpresaResponse, ListadoActivableRequest>(empresaRepository), IEmpresaService
{
    private readonly IEmpresaRepository _empresaRepository = empresaRepository;

    public async Task<EmpresaResponse> CrearAsync(CrearEmpresaRequest request)
    {
        await ValidarCuitAsync(request.CUIT, null);

        var empresa = new Empresa(request.RazonSocial, request.CUIT, request.Telefono);

        await _empresaRepository.AddAsync(empresa);

        return EmpresaMapper.ToResponse(empresa);
    }

    public async Task<EmpresaResponse> ObtenerPorIdAsync(int id)
    {
        var empresa = await ObtenerEmpresaById(id);

        return EmpresaMapper.ToResponse(empresa);
    }

    public async Task<List<EmpresaResponse>> ObtenerTodosAsync()
    {
        var empresas = await _empresaRepository.GetAllConInactivasAsync();

        return EmpresaMapper.ToResponse(empresas);
    }

    public async Task<EmpresaResponse> ActualizarAsync(int id, ActualizarEmpresaRequest request)
    {
        var empresa = await ObtenerEmpresaById(id);

        await ValidarCuitAsync(request.CUIT, id);

        empresa.ActualizarDatos(request.RazonSocial, request.CUIT, request.Telefono);

        await _empresaRepository.UpdateAsync(empresa);

        return EmpresaMapper.ToResponse(empresa);
    }

    public async Task EliminarAsync(int id)
    {
        var empresa = await ObtenerEmpresaById(id);

        empresa.Desactivar();

        await _empresaRepository.DeleteAsync(empresa);
    }

    protected override List<EmpresaResponse> Convertir(IEnumerable<Empresa> entidades)
        => EmpresaMapper.ToResponse(entidades);

    private async Task ValidarCuitAsync(string cuit, int? idExcluido)
    {
        if (await _empresaRepository.ExisteCuitAsync(cuit, idExcluido))
            throw new InvalidOperationException("Ya existe una empresa con ese CUIT.");
    }

    private async Task<Empresa> ObtenerEmpresaById(int id)
    {
        Empresa? empresa = await _empresaRepository.GetByIdConInactivasAsync(id);

        return empresa ?? throw new EmpresaNoEncontradaException(id);
    }
}
