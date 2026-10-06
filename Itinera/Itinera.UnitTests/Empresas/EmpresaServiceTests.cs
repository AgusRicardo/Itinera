using Itinera.Application.Common.Exceptions;
using Itinera.Application.Empresas.Dtos;
using Itinera.Application.Empresas.Interfaces;
using Itinera.Application.Empresas.Services;
using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.UnitTests.Empresas;

public class EmpresaServiceTests
{
    [Fact]
    public async Task CrearAsync_persiste_y_devuelve_la_empresa()
    {
        var repositorio = new EmpresaRepositoryFake();
        var servicio = new EmpresaService(repositorio);

        var response = await servicio.CrearAsync(new CrearEmpresaRequest
        {
            RazonSocial = "Viajes SA",
            CUIT = "30-12345678-9",
            Telefono = "1122334455"
        });

        Assert.True(response.Id > 0);
        Assert.Equal("Viajes SA", response.RazonSocial);
        Assert.Equal("30-12345678-9", response.CUIT);
        Assert.True(response.Activo);
        Assert.Single(repositorio.Empresas);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_una_empresa_inexistente_lanza_excepcion()
    {
        var servicio = new EmpresaService(new EmpresaRepositoryFake());

        await Assert.ThrowsAsync<EmpresaNoEncontradaException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    [Fact]
    public async Task ActualizarAsync_cambia_los_datos()
    {
        var repositorio = new EmpresaRepositoryFake();
        var servicio = new EmpresaService(repositorio);
        var creada = await servicio.CrearAsync(new CrearEmpresaRequest
        {
            RazonSocial = "Viajes SA",
            CUIT = "30-12345678-9",
            Telefono = "1122334455"
        });

        var response = await servicio.ActualizarAsync(creada.Id, new ActualizarEmpresaRequest
        {
            RazonSocial = "Turismo SA",
            CUIT = "30-98765432-1",
            Telefono = "1199887766"
        });

        Assert.Equal("Turismo SA", response.RazonSocial);
        Assert.Equal("30-98765432-1", response.CUIT);
    }

    [Fact]
    public async Task EliminarAsync_desactiva_la_empresa()
    {
        var repositorio = new EmpresaRepositoryFake();
        var servicio = new EmpresaService(repositorio);
        var creada = await servicio.CrearAsync(new CrearEmpresaRequest
        {
            RazonSocial = "Viajes SA",
            CUIT = "30-12345678-9",
            Telefono = "1122334455"
        });

        await servicio.EliminarAsync(creada.Id);

        Assert.False(repositorio.Empresas.Single().Activo);
    }

    [Fact]
    public async Task ObtenerTodosAsync_devuelve_las_empresas_creadas()
    {
        var servicio = new EmpresaService(new EmpresaRepositoryFake());
        await servicio.CrearAsync(new CrearEmpresaRequest { RazonSocial = "Viajes SA", CUIT = "30-12345678-9" });
        await servicio.CrearAsync(new CrearEmpresaRequest { RazonSocial = "Turismo SA", CUIT = "30-98765432-1" });

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    private sealed class EmpresaRepositoryFake : IEmpresaRepository
    {
        private int _ultimoId;

        public List<EmpresaEntidad> Empresas { get; } = new();

        public Task AddAsync(EmpresaEntidad empresa)
        {
            _ultimoId++;
            typeof(EmpresaEntidad).GetProperty(nameof(EmpresaEntidad.Id))!.SetValue(empresa, _ultimoId);
            Empresas.Add(empresa);
            return Task.CompletedTask;
        }

        public Task<EmpresaEntidad?> GetByIdAsync(int id)
            => Task.FromResult(Empresas.FirstOrDefault(e => e.Id == id));

        public Task<List<EmpresaEntidad>> GetAllAsync()
            => Task.FromResult(Empresas.ToList());

        public Task UpdateAsync(EmpresaEntidad empresa) => Task.CompletedTask;

        public Task DeleteAsync(EmpresaEntidad empresa) => Task.CompletedTask;
    }
}
