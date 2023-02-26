using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class FreeLoadFactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FreeLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    DriverFee = table.Column<double>(type: "float", nullable: false),
                    Tonnage = table.Column<double>(type: "float", nullable: true),
                    TonnagePrice = table.Column<double>(type: "float", nullable: true),
                    DriverTonnagePrice = table.Column<double>(type: "float", nullable: true),
                    LoadNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LoadNumberGov = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VAT = table.Column<double>(type: "float", nullable: false),
                    LoadFactorDeductions = table.Column<double>(type: "float", nullable: false),
                    WithholdingTax = table.Column<double>(type: "float", nullable: false),
                    CreateDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreeLoadFactor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FreeLoadFactor_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FreeLoadFactor_CalendarId",
                schema: "dbo",
                table: "FreeLoadFactor",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_FreeLoadFactor_RowId",
                schema: "dbo",
                table: "FreeLoadFactor",
                column: "RowId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FreeLoadFactor",
                schema: "dbo");
        }
    }
}
