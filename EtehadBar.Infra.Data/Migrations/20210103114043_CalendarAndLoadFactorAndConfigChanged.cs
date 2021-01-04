using Microsoft.EntityFrameworkCore.Migrations;

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CalendarAndLoadFactorAndConfigChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "VAT",
                schema: "dbo",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "WithholdingTax",
                schema: "dbo",
                table: "Calendar");

            migrationBuilder.AddColumn<double>(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "LoadFactor",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VAT",
                schema: "dbo",
                table: "LoadFactor",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WithholdingTax",
                schema: "dbo",
                table: "LoadFactor",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "Config",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VAT",
                schema: "dbo",
                table: "Config",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WithholdingTax",
                schema: "dbo",
                table: "Config",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "VAT",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "WithholdingTax",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "VAT",
                schema: "dbo",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "WithholdingTax",
                schema: "dbo",
                table: "Config");

            migrationBuilder.AddColumn<double>(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "Calendar",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VAT",
                schema: "dbo",
                table: "Calendar",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WithholdingTax",
                schema: "dbo",
                table: "Calendar",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
