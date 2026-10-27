using Itinera.Domain.Usuarios;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.Domain.Common;

public class EmpresaCliente
{
    public int EmpresaId { get; private set; }
    public int ClienteId { get; private set; }
    public bool Activo { get; private set; } = true;
    public DateTime FechaRelacion { get; private set; }

    public EmpresaEntidad Empresa { get; private set; } = null!;
    public Cliente Cliente { get; private set; } = null!;

    private EmpresaCliente()
    {
    }

    public EmpresaCliente(int empresaId, int clienteId)
    {
        EmpresaId = empresaId;
        ClienteId = clienteId;
        FechaRelacion = DateTime.UtcNow;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
