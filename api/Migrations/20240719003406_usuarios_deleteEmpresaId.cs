using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class usuarios_deleteEmpresaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_User_Empresas_Empresaid", table: "User");

            migrationBuilder.DropIndex(name: "IX_User_Empresaid", table: "User");

            migrationBuilder.DropColumn(name: "Empresaid", table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Empresaid",
                table: "User",
                type: "int",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_User_Empresaid",
                table: "User",
                column: "Empresaid"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_User_Empresas_Empresaid",
                table: "User",
                column: "Empresaid",
                principalTable: "Empresas",
                principalColumn: "id"
            );
        }
    }
}
