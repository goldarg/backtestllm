using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class TicketAgregarPropiedades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "departamento", table: "Tickets");

            migrationBuilder.DropColumn(name: "email", table: "Tickets");

            migrationBuilder.DropColumn(name: "nombreCompleto", table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "telefono",
                table: "Tickets",
                newName: "departamentoCrmId"
            );

            migrationBuilder.AddColumn<string>(
                name: "dominioCrmId",
                table: "Tickets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "dominioCrmId", table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "departamentoCrmId",
                table: "Tickets",
                newName: "telefono"
            );

            migrationBuilder.AddColumn<string>(
                name: "departamento",
                table: "Tickets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Tickets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "nombreCompleto",
                table: "Tickets",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: ""
            );
        }
    }
}
