using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerFactorCalendar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CalendarId",
                schema: "dbo",
                table: "CustomerFactor",
                type: "bigint",
                nullable: false,
                defaultValue: 2L);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFactor_CalendarId",
                schema: "dbo",
                table: "CustomerFactor",
                column: "CalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFactor_Calendar_CalendarId",
                schema: "dbo",
                table: "CustomerFactor",
                column: "CalendarId",
                principalSchema: "dbo",
                principalTable: "Calendar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFactor_Calendar_CalendarId",
                schema: "dbo",
                table: "CustomerFactor");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFactor_CalendarId",
                schema: "dbo",
                table: "CustomerFactor");

            migrationBuilder.DropColumn(
                name: "CalendarId",
                schema: "dbo",
                table: "CustomerFactor");
        }
    }
}
