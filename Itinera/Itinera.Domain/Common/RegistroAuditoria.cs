namespace Itinera.Domain.Common;

public sealed class RegistroAuditoria
{
    public int Id { get; private set; }
    public Guid? UsuarioId { get; private set; }
    public int? EmpresaId { get; private set; }
    public string Accion { get; private set; }
    public string Entidad { get; private set; }
    public int EntidadId { get; private set; }
    public string Detalle { get; private set; }
    public DateTime Fecha { get; private set; }

    private RegistroAuditoria()
    {
        Accion = string.Empty;
        Entidad = string.Empty;
    }

    public RegistroAuditoria(
        Guid? usuarioId,
        int? empresaId,
        AccionAuditoria accion,
        string entidad,
        int entidadId,
        string detalle)
    {
        if (!Enum.IsDefined(accion))
            throw new ArgumentException("La acción es obligatoria.", nameof(accion));
        if (string.IsNullOrWhiteSpace(entidad))
            throw new ArgumentException("La entidad es obligatoria.", nameof(entidad));

        UsuarioId = usuarioId;
        EmpresaId = empresaId;
        Accion = accion.ToString();
        Entidad = entidad;
        EntidadId = entidadId;
        Detalle = detalle;
        Fecha = DateTime.UtcNow;
    }
}
