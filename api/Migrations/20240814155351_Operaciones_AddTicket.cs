using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Operaciones_AddTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreCompleto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    empresaId = table.Column<int>(type: "int", nullable: false),
                    dominio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    departamento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tipoOperacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    asunto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    zona = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    odometro = table.Column<int>(type: "int", nullable: false),
                    turnoOpcion1 = table.Column<DateTime>(type: "datetime", nullable: false),
                    turnoOpcion2 = table.Column<DateTime>(type: "datetime", nullable: false),
                    idTiquetera = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    numeroTicket = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tickets_Empresas_empresaId",
                        column: x => x.empresaId,
                        principalTable: "Empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_empresaId",
                table: "Tickets",
                column: "empresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
