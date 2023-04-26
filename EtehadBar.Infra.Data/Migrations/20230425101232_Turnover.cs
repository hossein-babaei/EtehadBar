using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class Turnover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaied",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReceived",
                schema: "dbo",
                table: "FreeLoadFactor",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Turnover",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Debtor = table.Column<double>(type: "float", nullable: false),
                    Creditor = table.Column<double>(type: "float", nullable: false),
                    TurnoverType = table.Column<int>(type: "int", nullable: false),
                    CreateDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turnover", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turnover_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Turnover_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turnover_CalendarId",
                schema: "dbo",
                table: "Turnover",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnover_RowId",
                schema: "dbo",
                table: "Turnover",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turnover_UserId",
                schema: "dbo",
                table: "Turnover",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Turnover",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "IsPaied",
                schema: "dbo",
                table: "FreeLoadFactor");

            migrationBuilder.DropColumn(
                name: "IsReceived",
                schema: "dbo",
                table: "FreeLoadFactor");
        }
    }
}
