using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itinera.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ItinerarioIaEIntensidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantidadViajeros",
                table: "Propuestas",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaFin",
                table: "Propuestas",
                type: "date",
                nullable: false,
                defaultValueSql: "CURRENT_DATE");

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaInicio",
                table: "Propuestas",
                type: "date",
                nullable: false,
                defaultValueSql: "CURRENT_DATE");

            migrationBuilder.AddColumn<int>(
                name: "IntensidadViajeId",
                table: "Propuestas",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "Preferencias",
                table: "Propuestas",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadViajeros",
                table: "Propuestas");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "Propuestas");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "Propuestas");

            migrationBuilder.DropColumn(
                name: "IntensidadViajeId",
                table: "Propuestas");

            migrationBuilder.DropColumn(
                name: "Preferencias",
                table: "Propuestas");
        }
    }
}
