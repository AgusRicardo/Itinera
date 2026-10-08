using Itinera.Domain.Common;
using Itinera.Domain.Propuestas;

namespace Itinera.Domain.Usuarios;

public class Cliente : Persona
{
    public List<Propuesta> Propuestas { get; private set; } = new();

    private Cliente()
    {
    }

    public Cliente(
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
}
