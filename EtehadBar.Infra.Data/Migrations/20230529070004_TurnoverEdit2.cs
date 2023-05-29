using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class TurnoverEdit2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnover_AspNetUsers_UserId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.DropIndex(
                name: "IX_Turnover_UserId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "dbo",
                table: "Turnover",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "dbo",
                table: "Turnover",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turnover_UserId",
                schema: "dbo",
                table: "Turnover",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnover_AspNetUsers_UserId",
                schema: "dbo",
                table: "Turnover",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
