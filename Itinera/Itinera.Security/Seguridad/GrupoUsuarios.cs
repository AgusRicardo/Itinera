namespace Itinera.Security.Domain;

public class GrupoUsuarios : UsuarioComponent
{
    private readonly List<GrupoMiembro> _grupoMiembros = new();

    public IReadOnlyList<GrupoMiembro> GrupoMiembros => _grupoMiembros.AsReadOnly();

    public GrupoUsuarios(string nombre)
        : base(nombre) { }

    private GrupoUsuarios() { }

    public void Agregar(Usuario usuario)
    {
        if (!_grupoMiembros.Any(gm => gm.MiembroId == usuario.Id))
            _grupoMiembros.Add(new GrupoMiembro(this, usuario));
    }

    public void Quitar(Usuario usuario)
    {
        var miembro = _grupoMiembros.FirstOrDefault(gm => gm.MiembroId == usuario.Id);
        if (miembro is not null)
            _grupoMiembros.Remove(miembro);
    }

    public new IReadOnlyList<Usuario> ObtenerMiembros()
        => _grupoMiembros.Select(gm => gm.Miembro).ToList().AsReadOnly();
}
