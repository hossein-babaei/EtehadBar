using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class LoadFactorDeduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "Config");

            migrationBuilder.AddColumn<double>(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "Customer",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1L,
                column: "LoadFactorDeductions",
                value: 5.0);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2L,
                column: "LoadFactorDeductions",
                value: 5.0);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3L,
                column: "LoadFactorDeductions",
                value: 7.8);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 4L,
                column: "LoadFactorDeductions",
                value: 5.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.AddColumn<double>(
                name: "LoadFactorDeductions",
                schema: "dbo",
                table: "Config",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "LoadFactorDeductions",
                value: 5.0);
        }
    }
}
