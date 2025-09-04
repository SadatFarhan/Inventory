using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Migrations
{
    /// <inheritdoc />
    public partial class users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "User",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "NameOrEmail",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameOrEmail",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "User",
                newName: "Username");
        }
    }
}
