using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class MehrcomHourlyAndCustomerPeriodicAddon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasLoadHours",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "LoadHours",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddonType",
                schema: "dbo",
                table: "CustomerPeriodicBalanceAddon",
                type: "int",
                nullable: false,
                defaultValue: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasLoadHours",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadHours",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "AddonType",
                schema: "dbo",
                table: "CustomerPeriodicBalanceAddon");
        }
    }
}
