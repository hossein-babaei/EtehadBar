using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class TurnoverProfile_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnover_TurnoverProfile_TurnoverProfileId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.DropColumn(
                name: "TurnoverType",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.AlterColumn<long>(
                name: "TurnoverProfileId",
                schema: "dbo",
                table: "Turnover",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnover_TurnoverProfile_TurnoverProfileId",
                schema: "dbo",
                table: "Turnover",
                column: "TurnoverProfileId",
                principalSchema: "dbo",
                principalTable: "TurnoverProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnover_TurnoverProfile_TurnoverProfileId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.AlterColumn<long>(
                name: "TurnoverProfileId",
                schema: "dbo",
                table: "Turnover",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "dbo",
                table: "Turnover",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TurnoverType",
                schema: "dbo",
                table: "Turnover",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnover_TurnoverProfile_TurnoverProfileId",
                schema: "dbo",
                table: "Turnover",
                column: "TurnoverProfileId",
                principalSchema: "dbo",
                principalTable: "TurnoverProfile",
                principalColumn: "Id");
        }
    }
}
