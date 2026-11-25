namespace Itinera.Domain.Facturacion;

public class ConceptoFactura
{
    public int Id { get; private set; }
    public string Descripcion { get; private set; }
    public decimal Precio { get; private set; }

    /// <summary>Si tiene valor, el concepto cobra ese porcentaje del costo del viaje en vez de un precio.</summary>
    public decimal? Porcentaje { get; private set; }
    public int EmpresaId { get; private set; }
    public bool Activo { get; private set; } = true;

    private ConceptoFactura()
    {
    }

    public ConceptoFactura(string descripcion, decimal precio, int empresaId, decimal? porcentaje = null)
    {
        if (empresaId <= 0)
            throw new ArgumentOutOfRangeException(nameof(empresaId));

        EmpresaId = empresaId;
        Actualizar(descripcion, precio, porcentaje);
    }

    public void Actualizar(string descripcion, decimal precio, decimal? porcentaje = null)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción del concepto es obligatoria.", nameof(descripcion));
        if (precio < 0)
            throw new ArgumentOutOfRangeException(nameof(precio), "El precio no puede ser negativo.");
        if (porcentaje is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(porcentaje), "El porcentaje debe ser mayor a 0 y hasta 100.");

        Descripcion = descripcion.Trim();
        Precio = porcentaje is null ? precio : 0;
        Porcentaje = porcentaje;
    }

    public void Desactivar() => Activo = false;
}
