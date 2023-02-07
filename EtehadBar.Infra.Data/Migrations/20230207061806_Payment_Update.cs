using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class Payment_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Vehicles_VehicleId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.AlterColumn<long>(
                name: "VehicleId",
                schema: "dbo",
                table: "Payment",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "dbo",
                table: "Payment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UserId",
                schema: "dbo",
                table: "Payment",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_AspNetUsers_UserId",
                schema: "dbo",
                table: "Payment",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Vehicles_VehicleId",
                schema: "dbo",
                table: "Payment",
                column: "VehicleId",
                principalSchema: "dbo",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_AspNetUsers_UserId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Vehicles_VehicleId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_UserId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.AlterColumn<long>(
                name: "VehicleId",
                schema: "dbo",
                table: "Payment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Vehicles_VehicleId",
                schema: "dbo",
                table: "Payment",
                column: "VehicleId",
                principalSchema: "dbo",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
