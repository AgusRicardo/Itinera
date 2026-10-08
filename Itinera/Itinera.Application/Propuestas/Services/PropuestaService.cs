using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Propuestas.Services;

public class PropuestaService : IPropuestaService
{
    private readonly IPropuestaRepository _propuestaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    public PropuestaService(
        IPropuestaRepository propuestaRepository,
        IClienteRepository clienteRepository,
        IEmpleadoRepository empleadoRepository
        )
    {
        _propuestaRepository = propuestaRepository;
        _clienteRepository = clienteRepository;
        _empleadoRepository = empleadoRepository;
    }
    public async Task<CrearPropuestaResponse> CrearAsync(CrearPropuestaRequest request)
    {
        Cliente cliente = await ObtenerClienteById(request.ClienteId);
        Empleado empleado = await ObtenerEmpleadoById(request.EmpleadoId);

        var propuesta = new Propuesta(
            cliente,
            empleado,
            request.Presupuesto);

        cliente.AsociarPropuesta(propuesta);
        empleado.AsociarPropuesta(propuesta);

        await _propuestaRepository.AddAsync(propuesta);

        return new CrearPropuestaResponse
        {
            Id = propuesta.Id
        };
    }

    private async Task<Empleado> ObtenerEmpleadoById(int empleadoId)
    {
        Empleado? empleado = await _empleadoRepository.GetByIdAsync(empleadoId);

        return empleado
            ?? throw new EmpleadoNoEncontradoException(empleadoId);

    }

    private async Task<Cliente> ObtenerClienteById(int clienteId)
    {
        Cliente? cliente = await _clienteRepository.GetByIdAsync(clienteId);

        return cliente
            ?? throw new ClienteNoEncontradoException(clienteId);
    }
}
