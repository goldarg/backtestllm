using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class addEmpresasPorUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Empresas_empresaId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "empresaId",
                table: "User",
                newName: "Empresaid");

            migrationBuilder.RenameIndex(
                name: "IX_User_empresaId",
                table: "User",
                newName: "IX_User_Empresaid");

            migrationBuilder.AlterColumn<int>(
                name: "Empresaid",
                table: "User",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "isRDA",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UsuariosEmpresas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    empresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosEmpresas", x => x.id);
                    table.ForeignKey(
                        name: "FK_UsuariosEmpresas_Empresas_empresaId",
                        column: x => x.empresaId,
                        principalTable: "Empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosEmpresas_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEmpresas_empresaId",
                table: "UsuariosEmpresas",
                column: "empresaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEmpresas_userId",
                table: "UsuariosEmpresas",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Empresas_Empresaid",
                table: "User",
                column: "Empresaid",
                principalTable: "Empresas",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Empresas_Empresaid",
                table: "User");

            migrationBuilder.DropTable(
                name: "UsuariosEmpresas");

            migrationBuilder.DropColumn(
                name: "isRDA",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "Empresaid",
                table: "User",
                newName: "empresaId");

            migrationBuilder.RenameIndex(
                name: "IX_User_Empresaid",
                table: "User",
                newName: "IX_User_empresaId");

            migrationBuilder.AlterColumn<int>(
                name: "empresaId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Empresas_empresaId",
                table: "User",
                column: "empresaId",
                principalTable: "Empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
