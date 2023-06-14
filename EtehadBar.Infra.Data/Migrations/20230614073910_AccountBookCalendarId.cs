using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class AccountBookCalendarId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CalendarId",
                schema: "dbo",
                table: "AccountBook",
                type: "bigint",
                nullable: false,
                defaultValue: 2L);

            migrationBuilder.CreateIndex(
                name: "IX_AccountBook_CalendarId",
                schema: "dbo",
                table: "AccountBook",
                column: "CalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBook_Calendar_CalendarId",
                schema: "dbo",
                table: "AccountBook",
                column: "CalendarId",
                principalSchema: "dbo",
                principalTable: "Calendar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBook_Calendar_CalendarId",
                schema: "dbo",
                table: "AccountBook");

            migrationBuilder.DropIndex(
                name: "IX_AccountBook_CalendarId",
                schema: "dbo",
                table: "AccountBook");

            migrationBuilder.DropColumn(
                name: "CalendarId",
                schema: "dbo",
                table: "AccountBook");
        }
    }
}
