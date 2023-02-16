using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class MehrcomParsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "ShippingFee",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LoadSleepPrice",
                schema: "dbo",
                table: "ShippingFee",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Load",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LoadNumberGovReturn",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "nvarchar(max)",
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

            migrationBuilder.AddColumn<bool>(
                name: "Palette",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Return",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "WeighbridgePrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasLoadSleep",
                schema: "dbo",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "HasAddonTonnage", "HasLoadSleep", "HasLoadType" },
                values: new object[] { true, true, true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "ShippingFee");

            migrationBuilder.DropColumn(
                name: "LoadSleepPrice",
                schema: "dbo",
                table: "ShippingFee");

            migrationBuilder.DropColumn(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "Load",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadNumberGovReturn",
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
                name: "Palette",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "Return",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "WeighbridgePrice",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "HasLoadSleep",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "HasAddonTonnage", "HasLoadType" },
                values: new object[] { false, false });
        }
    }
}
