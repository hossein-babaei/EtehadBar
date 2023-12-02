using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class LoadFactorNovin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoadFactorNovin",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsPaied = table.Column<bool>(type: "bit", nullable: false),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    DriverFee = table.Column<double>(type: "float", nullable: false),
                    Tonnage = table.Column<double>(type: "float", nullable: true),
                    TonnagePrice = table.Column<double>(type: "float", nullable: true),
                    DriverTonnagePrice = table.Column<double>(type: "float", nullable: true),
                    LoadNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LoadNumberGov = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Attachments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadFactorNovin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadFactorNovin_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadFactorNovin_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadFactorNovin_Driver_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "dbo",
                        principalTable: "Driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadFactorNovin_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "dbo",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactorNovin_CalendarId",
                schema: "dbo",
                table: "LoadFactorNovin",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactorNovin_CustomerId",
                schema: "dbo",
                table: "LoadFactorNovin",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactorNovin_DriverId",
                schema: "dbo",
                table: "LoadFactorNovin",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactorNovin_RowId",
                schema: "dbo",
                table: "LoadFactorNovin",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactorNovin_VehicleId",
                schema: "dbo",
                table: "LoadFactorNovin",
                column: "VehicleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoadFactorNovin",
                schema: "dbo");
        }
    }
}
