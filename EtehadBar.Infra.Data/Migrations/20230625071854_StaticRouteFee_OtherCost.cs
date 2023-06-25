using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class StaticRouteFee_OtherCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FakeLoadFactor",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "OtherCost",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LeftNumber = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    NumberWord = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    RightNumber = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IranStateNumber = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherCost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherCost_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaticRouteFee",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticRouteFee", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtherCost_CalendarId",
                schema: "dbo",
                table: "OtherCost",
                column: "CalendarId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtherCost",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "StaticRouteFee",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "FakeLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationId = table.Column<long>(type: "bigint", nullable: false),
                    OriginId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DriverFee = table.Column<double>(type: "float", nullable: false),
                    EditDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LoadNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FakeLoadFactor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FakeLoadFactor_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FakeLoadFactor_LoadRoute_DestinationId",
                        column: x => x.DestinationId,
                        principalSchema: "dbo",
                        principalTable: "LoadRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FakeLoadFactor_LoadRoute_OriginId",
                        column: x => x.OriginId,
                        principalSchema: "dbo",
                        principalTable: "LoadRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FakeLoadFactor_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "dbo",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FakeLoadFactor_CalendarId",
                schema: "dbo",
                table: "FakeLoadFactor",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_FakeLoadFactor_DestinationId",
                schema: "dbo",
                table: "FakeLoadFactor",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_FakeLoadFactor_OriginId",
                schema: "dbo",
                table: "FakeLoadFactor",
                column: "OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_FakeLoadFactor_RowId",
                schema: "dbo",
                table: "FakeLoadFactor",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FakeLoadFactor_VehicleId",
                schema: "dbo",
                table: "FakeLoadFactor",
                column: "VehicleId");
        }
    }
}
