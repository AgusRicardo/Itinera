using Itinera.Domain.Catalogos;

namespace Itinera.Domain.Propuestas;

public class PropuestaEtiqueta
{
    public int PropuestaId { get; private set; }
    public int EtiquetaId { get; private set; }
    public Propuesta Propuesta { get; private set; }
    public Etiqueta Etiqueta { get; private set; }

    private PropuestaEtiqueta()
    {
    }

    public PropuestaEtiqueta(Propuesta propuesta, Etiqueta etiqueta)
    {
        Propuesta = propuesta ?? throw new ArgumentNullException(nameof(propuesta));
        Etiqueta = etiqueta ?? throw new ArgumentNullException(nameof(etiqueta));
        PropuestaId = propuesta.Id;
        EtiquetaId = etiqueta.Id;
    }
}
