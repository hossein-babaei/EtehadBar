using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class VehicleBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleBalance",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    CalendarId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    LoadFactorId = table.Column<long>(type: "bigint", nullable: true),
                    BillId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    EditDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleBalance_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VehicleBalance_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "dbo",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBalance_CalendarId",
                schema: "dbo",
                table: "VehicleBalance",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBalance_RowId",
                schema: "dbo",
                table: "VehicleBalance",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBalance_VehicleId",
                schema: "dbo",
                table: "VehicleBalance",
                column: "VehicleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleBalance",
                schema: "dbo");
        }
    }
}
