using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itinera.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Multitenencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Propuestas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmpresaClientes",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaRelacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaClientes", x => new { x.EmpresaId, x.ClienteId });
                    table.ForeignKey(
                        name: "FK_EmpresaClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpresaClientes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosEmpleados",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpleadoId = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosEmpleados", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_UsuariosEmpleados_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosEmpleados_UsuariosSeguridad_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "UsuariosSeguridad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Propuestas_EmpresaId",
                table: "Propuestas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaClientes_ClienteId",
                table: "EmpresaClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEmpleados_EmpleadoId",
                table: "UsuariosEmpleados",
                column: "EmpleadoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpresaClientes");

            migrationBuilder.DropTable(
                name: "UsuariosEmpleados");

            migrationBuilder.DropIndex(
                name: "IX_Propuestas_EmpresaId",
                table: "Propuestas");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Propuestas");
        }
    }
}
