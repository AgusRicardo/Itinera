using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itinera.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PropuestaConVariosItinerarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Itinerarios_PropuestaId",
                table: "Itinerarios");

            migrationBuilder.DropColumn(
                name: "RecomendadaPorIA",
                table: "ActividadesDestinoItinerario");

            migrationBuilder.CreateTable(
                name: "PropuestasActividades",
                columns: table => new
                {
                    PropuestaId = table.Column<int>(type: "integer", nullable: false),
                    ActividadId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestasActividades", x => new { x.PropuestaId, x.ActividadId });
                    table.ForeignKey(
                        name: "FK_PropuestasActividades_Actividades_ActividadId",
                        column: x => x.ActividadId,
                        principalTable: "Actividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropuestasActividades_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropuestasDestinos",
                columns: table => new
                {
                    PropuestaId = table.Column<int>(type: "integer", nullable: false),
                    DestinoId = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestasDestinos", x => new { x.PropuestaId, x.DestinoId });
                    table.ForeignKey(
                        name: "FK_PropuestasDestinos_Destinos_DestinoId",
                        column: x => x.DestinoId,
                        principalTable: "Destinos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropuestasDestinos_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Itinerarios_PropuestaId",
                table: "Itinerarios",
                column: "PropuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasActividades_ActividadId",
                table: "PropuestasActividades",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasDestinos_DestinoId",
                table: "PropuestasDestinos",
                column: "DestinoId");

            migrationBuilder.Sql(
                """
                INSERT INTO "PropuestasDestinos" ("PropuestaId", "DestinoId", "Orden")
                SELECT i."PropuestaId", d."DestinoId", MIN(d."Orden")
                FROM "DestinosItinerario" d
                JOIN "Itinerarios" i ON i."Id" = d."ItinerarioId"
                GROUP BY i."PropuestaId", d."DestinoId";

                INSERT INTO "PropuestasActividades" ("PropuestaId", "ActividadId")
                SELECT DISTINCT i."PropuestaId", a."ActividadId"
                FROM "ActividadesDestinoItinerario" a
                JOIN "DestinosItinerario" d ON d."Id" = a."DestinoItinerarioId"
                JOIN "Itinerarios" i ON i."Id" = d."ItinerarioId"
                WHERE a."Observaciones" = 'Actividad seleccionada para la propuesta.';

                UPDATE "Propuestas" p
                SET "FechaInicio" = i."FechaInicio", "FechaFin" = i."FechaFin"
                FROM "Itinerarios" i
                WHERE i."PropuestaId" = p."Id";

                DELETE FROM "Itinerarios" i
                USING "Propuestas" p
                WHERE p."Id" = i."PropuestaId"
                  AND p."EstadoPropuestaId" = 1
                  AND i."Nombre" IS NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "DestinosItinerario" d
                      JOIN "ActividadesDestinoItinerario" a ON a."DestinoItinerarioId" = d."Id"
                      WHERE d."ItinerarioId" = i."Id"
                        AND a."Observaciones" IS DISTINCT FROM 'Actividad seleccionada para la propuesta.');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropuestasActividades");

            migrationBuilder.DropTable(
                name: "PropuestasDestinos");

            migrationBuilder.DropIndex(
                name: "IX_Itinerarios_PropuestaId",
                table: "Itinerarios");

            migrationBuilder.AddColumn<bool>(
                name: "RecomendadaPorIA",
                table: "ActividadesDestinoItinerario",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Itinerarios_PropuestaId",
                table: "Itinerarios",
                column: "PropuestaId",
                unique: true);
        }
    }
}
