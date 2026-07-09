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

    public Empleado(
        string nombre,
        string apellido,
        string email,
        string telefono
    ) : base(nombre, apellido, email, telefono)
    {
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
