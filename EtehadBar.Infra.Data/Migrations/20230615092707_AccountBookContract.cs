using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class AccountBookContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ContractId",
                schema: "dbo",
                table: "CustomerIncome",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<long>(
                name: "ContractId",
                schema: "dbo",
                table: "AccountBook",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIncome_ContractId",
                schema: "dbo",
                table: "CustomerIncome",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBook_ContractId",
                schema: "dbo",
                table: "AccountBook",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBook_Contract_ContractId",
                schema: "dbo",
                table: "AccountBook",
                column: "ContractId",
                principalSchema: "dbo",
                principalTable: "Contract",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerIncome_Contract_ContractId",
                schema: "dbo",
                table: "CustomerIncome",
                column: "ContractId",
                principalSchema: "dbo",
                principalTable: "Contract",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBook_Contract_ContractId",
                schema: "dbo",
                table: "AccountBook");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerIncome_Contract_ContractId",
                schema: "dbo",
                table: "CustomerIncome");

            migrationBuilder.DropIndex(
                name: "IX_CustomerIncome_ContractId",
                schema: "dbo",
                table: "CustomerIncome");

            migrationBuilder.DropIndex(
                name: "IX_AccountBook_ContractId",
                schema: "dbo",
                table: "AccountBook");

            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "dbo",
                table: "CustomerIncome");

            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "dbo",
                table: "AccountBook");
        }
    }
}
