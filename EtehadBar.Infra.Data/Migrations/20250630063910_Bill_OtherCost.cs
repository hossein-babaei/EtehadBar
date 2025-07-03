using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class Bill_OtherCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BillId",
                schema: "dbo",
                table: "OtherCost",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtherCost_BillId",
                schema: "dbo",
                table: "OtherCost",
                column: "BillId");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherCost_Bill_BillId",
                schema: "dbo",
                table: "OtherCost",
                column: "BillId",
                principalSchema: "dbo",
                principalTable: "Bill",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherCost_Bill_BillId",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropIndex(
                name: "IX_OtherCost_BillId",
                schema: "dbo",
                table: "OtherCost");

            migrationBuilder.DropColumn(
                name: "BillId",
                schema: "dbo",
                table: "OtherCost");
        }
    }
}
