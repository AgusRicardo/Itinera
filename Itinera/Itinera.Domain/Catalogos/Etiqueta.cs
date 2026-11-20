namespace Itinera.Domain.Catalogos;

public class Etiqueta
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public int EmpresaId { get; private set; }
    public bool Activo { get; private set; } = true;

    private Etiqueta()
    {
    }

    public Etiqueta(string nombre, int empresaId)
    {
        Validar(nombre, empresaId);
        Nombre = nombre.Trim();
        EmpresaId = empresaId;
    }

    public void Actualizar(string nombre)
    {
        Validar(nombre, EmpresaId);
        Nombre = nombre.Trim();
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    private static void Validar(string nombre, int empresaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la etiqueta es obligatorio.", nameof(nombre));
        if (empresaId <= 0)
            throw new ArgumentOutOfRangeException(nameof(empresaId));
    }
}
