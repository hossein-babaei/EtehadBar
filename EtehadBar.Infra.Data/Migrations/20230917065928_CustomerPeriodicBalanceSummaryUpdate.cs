using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerPeriodicBalanceSummaryUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "InsuranceBalanceAmount",
                schema: "dbo",
                table: "CustomerPeriodicBalanceSummary",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsuranceBalanceAmount",
                schema: "dbo",
                table: "CustomerPeriodicBalanceSummary");
        }
    }
}
