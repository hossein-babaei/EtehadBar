using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class TurnoverProfilePeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfitPercent",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TurnoverProfilePeriod",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TurnoverProfileId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnoverProfilePeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnoverProfilePeriod_TurnoverProfile_TurnoverProfileId",
                        column: x => x.TurnoverProfileId,
                        principalSchema: "dbo",
                        principalTable: "TurnoverProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "Year",
                value: "1403");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoverProfilePeriod_RowId",
                schema: "dbo",
                table: "TurnoverProfilePeriod",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TurnoverProfilePeriod_TurnoverProfileId",
                schema: "dbo",
                table: "TurnoverProfilePeriod",
                column: "TurnoverProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TurnoverProfilePeriod",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "ProfitPercent",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "Year",
                value: "1402");
        }
    }
}
