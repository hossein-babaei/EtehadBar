using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CostUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CostAccountId",
                schema: "dbo",
                table: "Cost",
                type: "bigint",
                nullable: false,
                defaultValue: 39L);

            migrationBuilder.CreateIndex(
                name: "IX_Cost_CostAccountId",
                schema: "dbo",
                table: "Cost",
                column: "CostAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cost_Definition_CostAccountId",
                schema: "dbo",
                table: "Cost",
                column: "CostAccountId",
                principalSchema: "dbo",
                principalTable: "Definition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cost_Definition_CostAccountId",
                schema: "dbo",
                table: "Cost");

            migrationBuilder.DropIndex(
                name: "IX_Cost_CostAccountId",
                schema: "dbo",
                table: "Cost");

            migrationBuilder.DropColumn(
                name: "CostAccountId",
                schema: "dbo",
                table: "Cost");
        }
    }
}
