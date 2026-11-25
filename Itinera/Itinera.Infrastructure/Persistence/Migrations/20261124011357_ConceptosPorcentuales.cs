using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itinera.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConceptosPorcentuales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Porcentaje",
                table: "DetallesFactura",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Porcentaje",
                table: "ConceptosFactura",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Porcentaje",
                table: "DetallesFactura");

            migrationBuilder.DropColumn(
                name: "Porcentaje",
                table: "ConceptosFactura");
        }
    }
}
