using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class VehicleUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBankName",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                schema: "dbo",
                table: "Vehicles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountBankName",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }
    }
}
