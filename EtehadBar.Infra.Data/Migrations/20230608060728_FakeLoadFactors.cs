using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class FakeLoadFactors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment",
                schema: "dbo");

            migrationBuilder.AddColumn<bool>(
                name: "RealStatus",
                schema: "dbo",
                table: "LoadRoute",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FakeLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationId = table.Column<long>(type: "bigint", nullable: false),
                    DriverFee = table.Column<double>(type: "float", nullable: false),
                    LoadNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FakeLoadFactor",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "RealStatus",
                schema: "dbo",
                table: "LoadRoute");

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VehicleId = table.Column<long>(type: "bigint", nullable: true),
                    AdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Payment_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "dbo",
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CalendarId",
                schema: "dbo",
                table: "Payment",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_RowId",
                schema: "dbo",
                table: "Payment",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UserId",
                schema: "dbo",
                table: "Payment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_VehicleId",
                schema: "dbo",
                table: "Payment",
                column: "VehicleId");
        }
    }
}
