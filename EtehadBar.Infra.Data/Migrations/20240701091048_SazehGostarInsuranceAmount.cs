using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class SazehGostarInsuranceAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "InsuranceAmount",
                schema: "dbo",
                table: "SazehGostarLoadFactor",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsuranceAmount",
                schema: "dbo",
                table: "SazehGostarLoadFactor");
        }
    }
}
