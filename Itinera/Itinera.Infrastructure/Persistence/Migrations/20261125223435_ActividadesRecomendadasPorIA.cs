using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itinera.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ActividadesRecomendadasPorIA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RecomendadaPorIA",
                table: "ActividadesDestinoItinerario",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE "ActividadesDestinoItinerario" AS item
                SET "RecomendadaPorIA" = TRUE
                FROM "Actividades" AS actividad
                WHERE actividad."Id" = item."ActividadId" AND actividad."GeneradoPorIA";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecomendadaPorIA",
                table: "ActividadesDestinoItinerario");
        }
    }
}
