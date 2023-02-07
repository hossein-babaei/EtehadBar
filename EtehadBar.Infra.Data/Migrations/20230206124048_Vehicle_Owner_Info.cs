using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class Vehicle_Owner_Info : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountBankName",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleOwnerFullname",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBankName",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleOwnerFullname",
                schema: "dbo",
                table: "Vehicles");
        }
    }
}
