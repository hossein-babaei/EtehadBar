using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class VehicleIntelligentCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverCardNo",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleCardNo",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverCardNo",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleCardNo",
                schema: "dbo",
                table: "Vehicles");
        }
    }
}
