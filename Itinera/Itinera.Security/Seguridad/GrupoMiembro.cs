namespace Itinera.Security.Domain;

public class GrupoMiembro
{
    public Guid GrupoId { get; private set; }
    public Guid MiembroId { get; private set; }
    public GrupoUsuarios Grupo { get; private set; }
    public UsuarioComponent Miembro { get; private set; }

    public GrupoMiembro(GrupoUsuarios grupo, UsuarioComponent miembro)
    {
        GrupoId = grupo.Id;
        MiembroId = miembro.Id;
        Grupo = grupo;
        Miembro = miembro;
    }

    private GrupoMiembro() { }
}
