using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Itinera.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FacturacionYEtiquetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Facturas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE "Facturas" factura
                SET "EmpresaId" = propuesta."EmpresaId"
                FROM "Propuestas" propuesta
                WHERE factura."PropuestaId" = propuesta."Id";
                """);

            migrationBuilder.DropIndex(
                name: "IX_Facturas_Numero",
                table: "Facturas");

            migrationBuilder.CreateTable(
                name: "Etiquetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiquetas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecuenciasFactura",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    UltimoNumero = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecuenciasFactura", x => x.EmpresaId);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO "SecuenciasFactura" ("EmpresaId", "UltimoNumero")
                SELECT "EmpresaId",
                       MAX(CAST(SUBSTRING("Numero" FROM 5) AS integer))
                FROM "Facturas"
                WHERE "Numero" LIKE 'FAC-%'
                GROUP BY "EmpresaId";
                """);

            migrationBuilder.CreateTable(
                name: "PropuestasEtiquetas",
                columns: table => new
                {
                    PropuestaId = table.Column<int>(type: "integer", nullable: false),
                    EtiquetaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestasEtiquetas", x => new { x.PropuestaId, x.EtiquetaId });
                    table.ForeignKey(
                        name: "FK_PropuestasEtiquetas_Etiquetas_EtiquetaId",
                        column: x => x.EtiquetaId,
                        principalTable: "Etiquetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropuestasEtiquetas_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_EmpresaId_Numero",
                table: "Facturas",
                columns: new[] { "EmpresaId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Etiquetas_EmpresaId_Nombre",
                table: "Etiquetas",
                columns: new[] { "EmpresaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasEtiquetas_EtiquetaId",
                table: "PropuestasEtiquetas",
                column: "EtiquetaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropuestasEtiquetas");

            migrationBuilder.DropTable(
                name: "SecuenciasFactura");

            migrationBuilder.DropTable(
                name: "Etiquetas");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_EmpresaId_Numero",
                table: "Facturas");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_Numero",
                table: "Facturas",
                column: "Numero",
                unique: true);

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Facturas");
        }
    }
}
