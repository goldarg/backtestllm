using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Ticket_AddSolicitante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "solicitanteId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_solicitanteId",
                table: "Tickets",
                column: "solicitanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_User_solicitanteId",
                table: "Tickets",
                column: "solicitanteId",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_User_solicitanteId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_solicitanteId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "solicitanteId",
                table: "Tickets");
        }
    }
}
