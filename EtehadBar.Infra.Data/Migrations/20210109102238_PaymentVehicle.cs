using Microsoft.EntityFrameworkCore.Migrations;

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class PaymentVehicle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_AspNetUsers_ApplicationUserId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_ApplicationUserId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.AddColumn<string>(
                name: "VehicleId",
                schema: "dbo",
                table: "Payment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                schema: "dbo",
                table: "Config",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LoadFactorDeductions", "VAT", "WithholdingTax" },
                values: new object[] { 5.0, 9.0, 3.0 });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_VehicleId",
                schema: "dbo",
                table: "Payment",
                column: "VehicleId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Vehicles_VehicleId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_VehicleId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                schema: "dbo",
                table: "Payment",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                schema: "dbo",
                table: "Config",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LoadFactorDeductions", "VAT", "WithholdingTax" },
                values: new object[] { 0.0, 0.0, 0.0 });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ApplicationUserId",
                schema: "dbo",
                table: "Payment",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_AspNetUsers_ApplicationUserId",
                schema: "dbo",
                table: "Payment",
                column: "ApplicationUserId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
