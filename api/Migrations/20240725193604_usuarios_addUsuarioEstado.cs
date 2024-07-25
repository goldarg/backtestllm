using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class usuarios_addUsuarioEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activo",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEstadoid",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "estadoId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "telefono",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UsuarioEstado",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEstado", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_UsuarioEstadoid",
                table: "User",
                column: "UsuarioEstadoid");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UsuarioEstado_UsuarioEstadoid",
                table: "User",
                column: "UsuarioEstadoid",
                principalTable: "UsuarioEstado",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UsuarioEstado_UsuarioEstadoid",
                table: "User");

            migrationBuilder.DropTable(
                name: "UsuarioEstado");

            migrationBuilder.DropIndex(
                name: "IX_User_UsuarioEstadoid",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UsuarioEstadoid",
                table: "User");

            migrationBuilder.DropColumn(
                name: "estadoId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "telefono",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
