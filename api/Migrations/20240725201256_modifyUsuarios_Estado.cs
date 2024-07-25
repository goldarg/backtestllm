using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class modifyUsuarios_Estado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activo",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "estado",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "telefono",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estado",
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
