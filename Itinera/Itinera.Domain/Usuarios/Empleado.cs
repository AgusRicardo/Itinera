using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Propuestas;

namespace Itinera.Domain.Usuarios;

public class Empleado : Persona
{
    public int CargoId { get; private set; }
    public int EmpresaId { get; private set; }
    public Cargo Cargo { get; private set; }
    public List<Propuesta> Propuestas { get; private set; } = new();
    public Empresa.Empresa Empresa { get; private set; }

    private Empleado()
    {
    }

    public Empleado(
        string nombre,
        string apellido,
        string email,
        string telefono,
        Cargo cargo,
        Empresa.Empresa empresa
    ) : base(nombre, apellido, email, telefono)
    {
        AsignarCargo(cargo);
        AsignarEmpresa(empresa);
    }

    public void AsignarCargo(Cargo cargo)
    {
        ArgumentNullException.ThrowIfNull(cargo);

        Cargo = cargo;
        CargoId = cargo.Id;
    }

    public void AsignarEmpresa(Empresa.Empresa empresa)
    {
        ArgumentNullException.ThrowIfNull(empresa);

        Empresa = empresa;
        EmpresaId = empresa.Id;
    }

    public void ActualizarDatos(
        string nombre,
        string apellido,
        string email,
        string telefono,
        Cargo cargo,
        Empresa.Empresa empresa)
    {
        base.ActualizarDatos(nombre, apellido, email, telefono);
        AsignarCargo(cargo);
        AsignarEmpresa(empresa);
    }

    public void AsociarPropuesta(Propuesta propuesta)
    {
        Propuestas.Add(propuesta);
    }

    public void PresentarPropuesta(Propuesta propuesta)
    {
        propuesta.Presentar();
    }

    public void RegistrarRespuesta(Propuesta propuesta)
    {
        propuesta.Aceptar();
    }
}
