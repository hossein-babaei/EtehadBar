using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerIncome_CustomerFactorReUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ContractId",
                schema: "dbo",
                table: "CustomerIncome",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                schema: "dbo",
                table: "CustomerFactor",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "dbo",
                table: "CustomerFactor");

            migrationBuilder.AlterColumn<long>(
                name: "ContractId",
                schema: "dbo",
                table: "CustomerIncome",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
