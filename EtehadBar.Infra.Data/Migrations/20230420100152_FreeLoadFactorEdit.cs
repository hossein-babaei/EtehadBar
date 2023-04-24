using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class FreeLoadFactorEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleNumber",
                schema: "dbo",
                table: "FreeLoadFactor");

            migrationBuilder.AddColumn<string>(
                name: "DriverNationalNumber",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IranStateNumber",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LeftNumber",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoadFactorScan",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumberWord",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RightNumber",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverNationalNumber",
                schema: "dbo",
                table: "FreeLoadFactor");

            migrationBuilder.DropColumn(
                name: "IranStateNumber",
                schema: "dbo",
                table: "FreeLoadFactor");

            migrationBuilder.DropColumn(
                name: "LeftNumber",
                schema: "dbo",
                table: "FreeLoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadFactorScan",
                schema: "dbo",
                table: "FreeLoadFactor");

            migrationBuilder.DropColumn(
                name: "NumberWord",
                schema: "dbo",
                table: "FreeLoadFactor");

            migrationBuilder.DropColumn(
                name: "RightNumber",
                schema: "dbo",
                table: "FreeLoadFactor");

            migrationBuilder.AddColumn<string>(
                name: "VehicleNumber",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
