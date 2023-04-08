using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class TransferFee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TransferFee",
                schema: "dbo",
                table: "BankAccountBook",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "Year",
                value: "1402");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferFee",
                schema: "dbo",
                table: "BankAccountBook");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "Year",
                value: "1401");
        }
    }
}
