using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class ActividadUsuario_CreateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActividadUsuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false),
                    usuarioEjecutorId = table.Column<int>(type: "int", nullable: false),
                    usuarioAfectadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadUsuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_ActividadUsuarios_User_usuarioAfectadoId",
                        column: x => x.usuarioAfectadoId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActividadUsuarios_User_usuarioEjecutorId",
                        column: x => x.usuarioEjecutorId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadUsuarios_usuarioAfectadoId",
                table: "ActividadUsuarios",
                column: "usuarioAfectadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadUsuarios_usuarioEjecutorId",
                table: "ActividadUsuarios",
                column: "usuarioEjecutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActividadUsuarios");
        }
    }
}
