using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itinera.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SaldoFacturasAnuladas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""UPDATE "Facturas" SET "SaldoPendiente" = 0 WHERE "EstadoFacturaId" = 4;""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""UPDATE "Facturas" SET "SaldoPendiente" = "ImporteTotal" WHERE "EstadoFacturaId" = 4;""");
        }
    }
}
