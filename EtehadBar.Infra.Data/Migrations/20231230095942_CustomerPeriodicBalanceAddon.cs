using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerPeriodicBalanceAddon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerPeriodicBalanceAddon",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    IsPositive = table.Column<bool>(type: "bit", nullable: false),
                    CustomerPeriodicBalanceSummaryId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPeriodicBalanceAddon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPeriodicBalanceAddon_CustomerPeriodicBalanceSummary_CustomerPeriodicBalanceSummaryId",
                        column: x => x.CustomerPeriodicBalanceSummaryId,
                        principalSchema: "dbo",
                        principalTable: "CustomerPeriodicBalanceSummary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPeriodicBalanceAddon_CustomerPeriodicBalanceSummaryId",
                schema: "dbo",
                table: "CustomerPeriodicBalanceAddon",
                column: "CustomerPeriodicBalanceSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPeriodicBalanceAddon_RowId",
                schema: "dbo",
                table: "CustomerPeriodicBalanceAddon",
                column: "RowId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPeriodicBalanceAddon",
                schema: "dbo");
        }
    }
}
