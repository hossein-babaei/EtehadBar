using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class TurnoverEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnover_Calendar_CalendarId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.DropIndex(
                name: "IX_Turnover_CalendarId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.DropColumn(
                name: "CalendarId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                schema: "dbo",
                table: "Turnover",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachments",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.AddColumn<long>(
                name: "CalendarId",
                schema: "dbo",
                table: "Turnover",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Turnover_CalendarId",
                schema: "dbo",
                table: "Turnover",
                column: "CalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnover_Calendar_CalendarId",
                schema: "dbo",
                table: "Turnover",
                column: "CalendarId",
                principalSchema: "dbo",
                principalTable: "Calendar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
