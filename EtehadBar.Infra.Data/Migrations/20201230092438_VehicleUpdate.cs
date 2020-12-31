using Microsoft.EntityFrameworkCore.Migrations;

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class VehicleUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.AddColumn<string>(
                name: "IranStateNumber",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LeftNumber",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumberWord",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RightNumber",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IranStateNumber",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LeftNumber",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NumberWord",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "RightNumber",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
