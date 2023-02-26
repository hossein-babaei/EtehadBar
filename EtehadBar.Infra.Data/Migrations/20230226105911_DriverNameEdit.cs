using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class DriverNameEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Firstname",
                schema: "dbo",
                table: "Driver");

            migrationBuilder.RenameColumn(
                name: "Lastname",
                schema: "dbo",
                table: "Driver",
                newName: "Fullname");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fullname",
                schema: "dbo",
                table: "Driver",
                newName: "Lastname");

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                schema: "dbo",
                table: "Driver",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
