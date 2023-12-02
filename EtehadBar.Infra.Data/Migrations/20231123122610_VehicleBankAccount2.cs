using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class VehicleBankAccount2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleBankAccounts_Definition_BankId",
                schema: "dbo",
                table: "VehicleBankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleBankAccounts_Vehicles_VehicleId",
                schema: "dbo",
                table: "VehicleBankAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleBankAccounts",
                schema: "dbo",
                table: "VehicleBankAccounts");

            migrationBuilder.RenameTable(
                name: "VehicleBankAccounts",
                schema: "dbo",
                newName: "VehicleBankAccount",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleBankAccounts_VehicleId",
                schema: "dbo",
                table: "VehicleBankAccount",
                newName: "IX_VehicleBankAccount_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleBankAccounts_RowId",
                schema: "dbo",
                table: "VehicleBankAccount",
                newName: "IX_VehicleBankAccount_RowId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleBankAccounts_BankId",
                schema: "dbo",
                table: "VehicleBankAccount",
                newName: "IX_VehicleBankAccount_BankId");

            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                schema: "dbo",
                table: "VehicleBankAccount",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleBankAccount",
                schema: "dbo",
                table: "VehicleBankAccount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleBankAccount_Definition_BankId",
                schema: "dbo",
                table: "VehicleBankAccount",
                column: "BankId",
                principalSchema: "dbo",
                principalTable: "Definition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleBankAccount_Vehicles_VehicleId",
                schema: "dbo",
                table: "VehicleBankAccount",
                column: "VehicleId",
                principalSchema: "dbo",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleBankAccount_Definition_BankId",
                schema: "dbo",
                table: "VehicleBankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleBankAccount_Vehicles_VehicleId",
                schema: "dbo",
                table: "VehicleBankAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleBankAccount",
                schema: "dbo",
                table: "VehicleBankAccount");

            migrationBuilder.DropColumn(
                name: "Fullname",
                schema: "dbo",
                table: "VehicleBankAccount");

            migrationBuilder.RenameTable(
                name: "VehicleBankAccount",
                schema: "dbo",
                newName: "VehicleBankAccounts",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleBankAccount_VehicleId",
                schema: "dbo",
                table: "VehicleBankAccounts",
                newName: "IX_VehicleBankAccounts_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleBankAccount_RowId",
                schema: "dbo",
                table: "VehicleBankAccounts",
                newName: "IX_VehicleBankAccounts_RowId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleBankAccount_BankId",
                schema: "dbo",
                table: "VehicleBankAccounts",
                newName: "IX_VehicleBankAccounts_BankId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleBankAccounts",
                schema: "dbo",
                table: "VehicleBankAccounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleBankAccounts_Definition_BankId",
                schema: "dbo",
                table: "VehicleBankAccounts",
                column: "BankId",
                principalSchema: "dbo",
                principalTable: "Definition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleBankAccounts_Vehicles_VehicleId",
                schema: "dbo",
                table: "VehicleBankAccounts",
                column: "VehicleId",
                principalSchema: "dbo",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
