using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class BillCustomerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                schema: "dbo",
                table: "Bill",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CustomerId",
                schema: "dbo",
                table: "Bill",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_Customer_CustomerId",
                schema: "dbo",
                table: "Bill",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_Customer_CustomerId",
                schema: "dbo",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_CustomerId",
                schema: "dbo",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "dbo",
                table: "Bill");
        }
    }
}
