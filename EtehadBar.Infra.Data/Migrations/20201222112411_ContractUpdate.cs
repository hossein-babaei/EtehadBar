using Microsoft.EntityFrameworkCore.Migrations;

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class ContractUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_Customer_CustomerId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_CustomerId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.AddColumn<string>(
                name: "ContractId",
                schema: "dbo",
                table: "LoadFactor",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                schema: "dbo",
                table: "Contract",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "dbo",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                schema: "dbo",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "dbo",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_ContractId",
                schema: "dbo",
                table: "LoadFactor",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_CustomerId",
                schema: "dbo",
                table: "Contract",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Customer_CustomerId",
                schema: "dbo",
                table: "Contract",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_Contract_ContractId",
                schema: "dbo",
                table: "LoadFactor",
                column: "ContractId",
                principalSchema: "dbo",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Customer_CustomerId",
                schema: "dbo",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_Contract_ContractId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_ContractId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_Contract_CustomerId",
                schema: "dbo",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "dbo",
                table: "Contract");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                schema: "dbo",
                table: "LoadFactor",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "dbo",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                schema: "dbo",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "dbo",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_CustomerId",
                schema: "dbo",
                table: "LoadFactor",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_Customer_CustomerId",
                schema: "dbo",
                table: "LoadFactor",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
