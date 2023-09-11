using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class OtherCostUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IranStateNumber",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropColumn(
                name: "LeftNumber",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropColumn(
                name: "NumberWord",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropColumn(
                name: "RightNumber",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                schema: "dbo",
                table: "OtherCost",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "VehicleId",
                schema: "dbo",
                table: "OtherCost",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_OtherCost_CustomerId",
                schema: "dbo",
                table: "OtherCost",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherCost_VehicleId",
                schema: "dbo",
                table: "OtherCost",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherCost_Customer_CustomerId",
                schema: "dbo",
                table: "OtherCost",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherCost_Vehicles_VehicleId",
                schema: "dbo",
                table: "OtherCost",
                column: "VehicleId",
                principalSchema: "dbo",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherCost_Customer_CustomerId",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherCost_Vehicles_VehicleId",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropIndex(
                name: "IX_OtherCost_CustomerId",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropIndex(
                name: "IX_OtherCost_VehicleId",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.AddColumn<string>(
                name: "IranStateNumber",
                schema: "dbo",
                table: "OtherCost",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LeftNumber",
                schema: "dbo",
                table: "OtherCost",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumberWord",
                schema: "dbo",
                table: "OtherCost",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RightNumber",
                schema: "dbo",
                table: "OtherCost",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }
    }
}
