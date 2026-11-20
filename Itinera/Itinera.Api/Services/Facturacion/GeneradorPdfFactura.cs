using Itinera.Application.Facturacion.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Itinera.Api.Services.Facturacion;

public sealed class GeneradorPdfFactura
{
    public byte[] Generar(FacturaResponse factura)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(documento =>
        {
            documento.Page(pagina =>
            {
                pagina.Size(PageSizes.A4);
                pagina.Margin(40);
                pagina.DefaultTextStyle(estilo => estilo.FontSize(10));

                pagina.Header().Column(columna =>
                {
                    columna.Item().Text(factura.EmpresaNombre).Bold().FontSize(18);
                    columna.Item().Text("Factura interna").FontSize(11).FontColor(Colors.Grey.Darken2);
                });

                pagina.Content().PaddingTop(24).Column(columna =>
                {
                    columna.Spacing(14);
                    columna.Item().Row(fila =>
                    {
                        fila.RelativeItem().Column(datos =>
                        {
                            datos.Item().Text($"Factura {factura.Numero}").Bold().FontSize(16);
                            datos.Item().Text($"Propuesta #{factura.PropuestaId}");
                            datos.Item().Text($"Cliente: {factura.ClienteNombre}");
                        });
                        fila.ConstantItem(180).Column(datos =>
                        {
                            datos.Item().Text($"Emisión: {factura.FechaEmision:dd/MM/yyyy}");
                            datos.Item().Text($"Vencimiento: {factura.FechaVencimiento:dd/MM/yyyy}");
                            datos.Item().Text($"Estado: {factura.Estado}");
                        });
                    });

                    columna.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columnas =>
                        {
                            columnas.RelativeColumn(4);
                            columnas.RelativeColumn(1);
                            columnas.RelativeColumn(2);
                            columnas.RelativeColumn(2);
                        });

                        tabla.Header(encabezado =>
                        {
                            encabezado.Cell().Element(EstiloEncabezado).Text("Concepto");
                            encabezado.Cell().Element(EstiloEncabezado).AlignRight().Text("Cantidad");
                            encabezado.Cell().Element(EstiloEncabezado).AlignRight().Text("Precio");
                            encabezado.Cell().Element(EstiloEncabezado).AlignRight().Text("Subtotal");
                        });

                        foreach (var detalle in factura.Detalles)
                        {
                            tabla.Cell().Element(EstiloCelda).Text(detalle.Descripcion);
                            tabla.Cell().Element(EstiloCelda).AlignRight().Text(detalle.Cantidad.ToString("N2"));
                            tabla.Cell().Element(EstiloCelda).AlignRight().Text(detalle.PrecioUnitario.ToString("C2"));
                            tabla.Cell().Element(EstiloCelda).AlignRight().Text(detalle.SubTotal.ToString("C2"));
                        }
                    });

                    columna.Item().AlignRight().Column(totales =>
                    {
                        totales.Item().Text($"Total: {factura.ImporteTotal:C2}").Bold().FontSize(13);
                        totales.Item().Text($"Saldo pendiente: {factura.SaldoPendiente:C2}");
                    });

                    columna.Item().Text("Pagos registrados").Bold().FontSize(12);
                    if (factura.Pagos.Count == 0)
                    {
                        columna.Item().Text("No hay pagos registrados.");
                    }
                    else
                    {
                        foreach (var pago in factura.Pagos)
                        {
                            columna.Item().Text(
                                $"{pago.Fecha:dd/MM/yyyy} · {pago.MedioPago} · {pago.Importe:C2}");
                        }
                    }
                });

                pagina.Footer().AlignCenter().Text("Itinera");
            });
        }).GeneratePdf();
    }

    private static IContainer EstiloEncabezado(IContainer contenedor)
        => contenedor
            .Background(Colors.Grey.Lighten2)
            .BorderBottom(1)
            .Padding(5);

    private static IContainer EstiloCelda(IContainer contenedor)
        => contenedor
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);
}
