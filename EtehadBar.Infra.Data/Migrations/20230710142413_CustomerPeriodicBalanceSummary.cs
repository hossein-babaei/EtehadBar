using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerPeriodicBalanceSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerPeriodicBalanceSummary",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BalanceAmount = table.Column<double>(type: "float", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPeriodicBalanceSummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPeriodicBalanceSummary_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPeriodicBalanceSummary_CustomerId",
                schema: "dbo",
                table: "CustomerPeriodicBalanceSummary",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPeriodicBalanceSummary_RowId",
                schema: "dbo",
                table: "CustomerPeriodicBalanceSummary",
                column: "RowId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPeriodicBalanceSummary",
                schema: "dbo");
        }
    }
}
