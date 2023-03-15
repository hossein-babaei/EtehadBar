using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class LoadFactorEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadSleepPrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadSleepTime",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "WeighbridgePrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.AddColumn<double>(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "LoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LoadSleepPrice",
                schema: "dbo",
                table: "LoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LoadSleepTime",
                schema: "dbo",
                table: "LoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WeighbridgePrice",
                schema: "dbo",
                table: "LoadFactor",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadSleepPrice",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadSleepTime",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "WeighbridgePrice",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.AddColumn<double>(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LoadSleepPrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LoadSleepTime",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WeighbridgePrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "float",
                nullable: true);
        }
    }
}
