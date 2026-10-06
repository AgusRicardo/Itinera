using Itinera.Application.Cargos.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Empleados.Dtos;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Empleados.Mappers;
using Itinera.Application.Empresas.Interfaces;
using Itinera.Domain.Empresa;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Empleados.Services;

public class EmpleadoService(
    IEmpleadoRepository empleadoRepository,
    ICargoRepository cargoRepository,
    IEmpresaRepository empresaRepository) : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository = empleadoRepository;
    private readonly ICargoRepository _cargoRepository = cargoRepository;
    private readonly IEmpresaRepository _empresaRepository = empresaRepository;

    public async Task<EmpleadoResponse> CrearAsync(CrearEmpleadoRequest request)
    {
        Cargo cargo = await ObtenerCargoById(request.CargoId);
        Empresa empresa = await ObtenerEmpresaById(request.EmpresaId);

        var empleado = new Empleado(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            cargo,
            empresa);

        await _empleadoRepository.AddAsync(empleado);

        return EmpleadoMapper.ToResponse(empleado);
    }

    public async Task<EmpleadoResponse> ObtenerPorIdAsync(int id)
    {
        var empleado = await ObtenerEmpleadoById(id);

        return EmpleadoMapper.ToResponse(empleado);
    }

    public async Task<List<EmpleadoResponse>> ObtenerTodosAsync()
    {
        var empleados = await _empleadoRepository.GetAllAsync();

        return EmpleadoMapper.ToResponse(empleados);
    }

    public async Task<EmpleadoResponse> ActualizarAsync(int id, ActualizarEmpleadoRequest request)
    {
        var empleado = await ObtenerEmpleadoById(id);
        Cargo cargo = await ObtenerCargoById(request.CargoId);
        Empresa empresa = await ObtenerEmpresaById(request.EmpresaId);

        empleado.ActualizarDatos(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            cargo,
            empresa);

        await _empleadoRepository.UpdateAsync(empleado);

        return EmpleadoMapper.ToResponse(empleado);
    }

    public async Task EliminarAsync(int id)
    {
        var empleado = await ObtenerEmpleadoById(id);

        empleado.Desactivar();

        await _empleadoRepository.DeleteAsync(empleado);
    }

    private async Task<Empleado> ObtenerEmpleadoById(int id)
    {
        Empleado? empleado = await _empleadoRepository.GetByIdAsync(id);

        return empleado ?? throw new EmpleadoNoEncontradoException(id);
    }

    private async Task<Cargo> ObtenerCargoById(int cargoId)
    {
        Cargo? cargo = await _cargoRepository.GetByIdAsync(cargoId);

        return cargo ?? throw new CargoNoEncontradoException(cargoId);
    }

    private async Task<Empresa> ObtenerEmpresaById(int empresaId)
    {
        Empresa? empresa = await _empresaRepository.GetByIdAsync(empresaId);

        return empresa ?? throw new EmpresaNoEncontradaException(empresaId);
    }
}
