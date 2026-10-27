using Itinera.Domain.Common;

namespace Itinera.Application.Clientes.Interfaces;

public interface IEmpresaClienteRepository
{
    Task AddAsync(EmpresaCliente empresaCliente);
}
