using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itinera.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EtiquetasUnicasActivas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Etiquetas_EmpresaId_Nombre",
                table: "Etiquetas");

            migrationBuilder.CreateIndex(
                name: "IX_Etiquetas_EmpresaId_Nombre",
                table: "Etiquetas",
                columns: new[] { "EmpresaId", "Nombre" },
                unique: true,
                filter: "\"Activo\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Etiquetas_EmpresaId_Nombre",
                table: "Etiquetas");

            migrationBuilder.CreateIndex(
                name: "IX_Etiquetas_EmpresaId_Nombre",
                table: "Etiquetas",
                columns: new[] { "EmpresaId", "Nombre" },
                unique: true);
        }
    }
}
