using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerIncomeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerIncome_Calendar_CalendarId",
                schema: "dbo",
                table: "CustomerIncome");

            migrationBuilder.AlterColumn<long>(
                name: "CalendarId",
                schema: "dbo",
                table: "CustomerIncome",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                schema: "dbo",
                table: "CustomerIncome",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerIncome_Calendar_CalendarId",
                schema: "dbo",
                table: "CustomerIncome",
                column: "CalendarId",
                principalSchema: "dbo",
                principalTable: "Calendar",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerIncome_Calendar_CalendarId",
                schema: "dbo",
                table: "CustomerIncome");

            migrationBuilder.DropColumn(
                name: "BankName",
                schema: "dbo",
                table: "CustomerIncome");

            migrationBuilder.AlterColumn<long>(
                name: "CalendarId",
                schema: "dbo",
                table: "CustomerIncome",
                type: "bigint",
                maxLength: 50,
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerIncome_Calendar_CalendarId",
                schema: "dbo",
                table: "CustomerIncome",
                column: "CalendarId",
                principalSchema: "dbo",
                principalTable: "Calendar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
