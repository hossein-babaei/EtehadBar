using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerFactorRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFactor_Contract_ContractId",
                schema: "dbo",
                table: "CustomerFactor");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFactor_ContractId",
                schema: "dbo",
                table: "CustomerFactor");

            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "dbo",
                table: "CustomerFactor");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFactor_CustomerId",
                schema: "dbo",
                table: "CustomerFactor",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFactor_Customer_CustomerId",
                schema: "dbo",
                table: "CustomerFactor",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFactor_Customer_CustomerId",
                schema: "dbo",
                table: "CustomerFactor");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFactor_CustomerId",
                schema: "dbo",
                table: "CustomerFactor");

            migrationBuilder.AddColumn<long>(
                name: "ContractId",
                schema: "dbo",
                table: "CustomerFactor",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFactor_ContractId",
                schema: "dbo",
                table: "CustomerFactor",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFactor_Contract_ContractId",
                schema: "dbo",
                table: "CustomerFactor",
                column: "ContractId",
                principalSchema: "dbo",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
