namespace Itinera.Security.Domain;

public class GrupoUsuarios : UsuarioComponent
{
    private readonly List<GrupoMiembro> _grupoMiembros = new();

    public IReadOnlyList<GrupoMiembro> GrupoMiembros => _grupoMiembros.AsReadOnly();

    public GrupoUsuarios(string nombre)
        : base(nombre) { }

    private GrupoUsuarios() { }

    public override void Agregar(UsuarioComponent componente)
    {
        ArgumentNullException.ThrowIfNull(componente);

        if (componente.Id == Id)
            throw new InvalidOperationException("Un grupo no puede contenerse a sí mismo.");

        if (Alcanza(componente, Id))
            throw new InvalidOperationException(
                "No se puede agregar un componente que ya contiene a este grupo: se produciría un ciclo.");

        if (!_grupoMiembros.Any(gm => gm.MiembroId == componente.Id))
            _grupoMiembros.Add(new GrupoMiembro(this, componente));
    }

    public override void Quitar(UsuarioComponent componente)
    {
        ArgumentNullException.ThrowIfNull(componente);

        var miembro = _grupoMiembros.FirstOrDefault(gm => gm.MiembroId == componente.Id);
        if (miembro is not null)
            _grupoMiembros.Remove(miembro);
    }

    public override IReadOnlyList<UsuarioComponent> ObtenerMiembros()
        => _grupoMiembros.Select(gm => gm.Miembro).ToList().AsReadOnly();

    private static bool Alcanza(UsuarioComponent origen, Guid buscado)
    {
        if (origen.Id == buscado)
            return true;

        if (origen is not GrupoUsuarios grupo)
            return false;

        return grupo.ObtenerMiembros().Any(miembro => Alcanza(miembro, buscado));
    }
}
