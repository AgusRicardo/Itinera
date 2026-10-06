using Itinera.Application.Cargos.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Empleados.Dtos;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Empleados.Services;
using Itinera.Application.Empresas.Interfaces;
using Itinera.Domain.Empresa;
using Itinera.Domain.Usuarios;
using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.UnitTests.Empleados;

public class EmpleadoServiceTests
{
    [Fact]
    public async Task CrearAsync_persiste_y_devuelve_el_empleado()
    {
        var cargoRepositorio = new CargoRepositoryFake();
        var empresaRepositorio = new EmpresaRepositoryFake();
        var cargo = await CrearCargo(cargoRepositorio);
        var empresa = await CrearEmpresa(empresaRepositorio);
        var repositorio = new EmpleadoRepositoryFake();
        var servicio = new EmpleadoService(repositorio, cargoRepositorio, empresaRepositorio);

        var response = await servicio.CrearAsync(new CrearEmpleadoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juan@correo.com",
            Telefono = "1122334455",
            CargoId = cargo.Id,
            EmpresaId = empresa.Id
        });

        Assert.True(response.Id > 0);
        Assert.Equal("Juan", response.Nombre);
        Assert.Equal(cargo.Id, response.CargoId);
        Assert.Equal("Agente de viajes", response.CargoDescripcion);
        Assert.Equal(empresa.Id, response.EmpresaId);
        Assert.Equal("Viajes SA", response.EmpresaRazonSocial);
        Assert.Single(repositorio.Empleados);
    }

    [Fact]
    public async Task CrearAsync_con_cargo_inexistente_lanza_excepcion()
    {
        var servicio = new EmpleadoService(
            new EmpleadoRepositoryFake(),
            new CargoRepositoryFake(),
            new EmpresaRepositoryFake());

        await Assert.ThrowsAsync<CargoNoEncontradoException>(
            () => servicio.CrearAsync(new CrearEmpleadoRequest
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@correo.com",
                CargoId = 99,
                EmpresaId = 1
            }));
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_un_empleado_inexistente_lanza_excepcion()
    {
        var servicio = new EmpleadoService(
            new EmpleadoRepositoryFake(),
            new CargoRepositoryFake(),
            new EmpresaRepositoryFake());

        await Assert.ThrowsAsync<EmpleadoNoEncontradoException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    [Fact]
    public async Task ActualizarAsync_cambia_los_datos()
    {
        var cargoRepositorio = new CargoRepositoryFake();
        var empresaRepositorio = new EmpresaRepositoryFake();
        var cargo = await CrearCargo(cargoRepositorio);
        var empresa = await CrearEmpresa(empresaRepositorio);
        var servicio = new EmpleadoService(new EmpleadoRepositoryFake(), cargoRepositorio, empresaRepositorio);
        var creado = await servicio.CrearAsync(new CrearEmpleadoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juan@correo.com",
            Telefono = "1122334455",
            CargoId = cargo.Id,
            EmpresaId = empresa.Id
        });

        var response = await servicio.ActualizarAsync(creado.Id, new ActualizarEmpleadoRequest
        {
            Nombre = "Juan",
            Apellido = "Lopez",
            Email = "juan.lopez@correo.com",
            Telefono = "1199887766",
            CargoId = cargo.Id,
            EmpresaId = empresa.Id
        });

        Assert.Equal("Lopez", response.Apellido);
        Assert.Equal("juan.lopez@correo.com", response.Email);
    }

    [Fact]
    public async Task EliminarAsync_desactiva_el_empleado()
    {
        var cargoRepositorio = new CargoRepositoryFake();
        var empresaRepositorio = new EmpresaRepositoryFake();
        var cargo = await CrearCargo(cargoRepositorio);
        var empresa = await CrearEmpresa(empresaRepositorio);
        var repositorio = new EmpleadoRepositoryFake();
        var servicio = new EmpleadoService(repositorio, cargoRepositorio, empresaRepositorio);
        var creado = await servicio.CrearAsync(new CrearEmpleadoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juan@correo.com",
            Telefono = "1122334455",
            CargoId = cargo.Id,
            EmpresaId = empresa.Id
        });

        await servicio.EliminarAsync(creado.Id);

        Assert.False(repositorio.Empleados.Single().Activo);
    }

    [Fact]
    public async Task ObtenerTodosAsync_devuelve_los_empleados_creados()
    {
        var cargoRepositorio = new CargoRepositoryFake();
        var empresaRepositorio = new EmpresaRepositoryFake();
        var cargo = await CrearCargo(cargoRepositorio);
        var empresa = await CrearEmpresa(empresaRepositorio);
        var servicio = new EmpleadoService(new EmpleadoRepositoryFake(), cargoRepositorio, empresaRepositorio);

        await servicio.CrearAsync(new CrearEmpleadoRequest { Nombre = "Juan", Apellido = "Perez", Email = "juan@correo.com", CargoId = cargo.Id, EmpresaId = empresa.Id });
        await servicio.CrearAsync(new CrearEmpleadoRequest { Nombre = "Maria", Apellido = "Diaz", Email = "maria@correo.com", CargoId = cargo.Id, EmpresaId = empresa.Id });

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    private static async Task<Cargo> CrearCargo(CargoRepositoryFake repositorio)
    {
        var cargo = new Cargo("Agente de viajes");
        await repositorio.AddAsync(cargo);
        return cargo;
    }

    private static async Task<EmpresaEntidad> CrearEmpresa(EmpresaRepositoryFake repositorio)
    {
        var empresa = new EmpresaEntidad("Viajes SA", "30-12345678-9", "1122334455");
        await repositorio.AddAsync(empresa);
        return empresa;
    }

    private sealed class CargoRepositoryFake : ICargoRepository
    {
        private int _ultimoId;

        public List<Cargo> Cargos { get; } = new();

        public Task AddAsync(Cargo cargo)
        {
            _ultimoId++;
            typeof(Cargo).GetProperty(nameof(Cargo.Id))!.SetValue(cargo, _ultimoId);
            Cargos.Add(cargo);
            return Task.CompletedTask;
        }

        public Task<Cargo?> GetByIdAsync(int id)
            => Task.FromResult(Cargos.FirstOrDefault(c => c.Id == id));

        public Task<List<Cargo>> GetAllAsync()
            => Task.FromResult(Cargos.ToList());

        public Task UpdateAsync(Cargo cargo) => Task.CompletedTask;

        public Task DeleteAsync(Cargo cargo) => Task.CompletedTask;
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

    private sealed class EmpleadoRepositoryFake : IEmpleadoRepository
    {
        private int _ultimoId;

        public List<Empleado> Empleados { get; } = new();

        public Task AddAsync(Empleado empleado)
        {
            _ultimoId++;
            typeof(Empleado).GetProperty(nameof(Empleado.Id))!.SetValue(empleado, _ultimoId);
            Empleados.Add(empleado);
            return Task.CompletedTask;
        }

        public Task<Empleado?> GetByIdAsync(int id)
            => Task.FromResult(Empleados.FirstOrDefault(e => e.Id == id));

        public Task<List<Empleado>> GetAllAsync()
            => Task.FromResult(Empleados.ToList());

        public Task UpdateAsync(Empleado empleado) => Task.CompletedTask;

        public Task DeleteAsync(Empleado empleado) => Task.CompletedTask;
    }
}
