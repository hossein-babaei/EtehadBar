using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class ShippingFeeGroupAndRoute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ShippingFeeRouteId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                schema: "dbo",
                table: "LoadFactor",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShippingFeeGroup",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vehicle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    DriverPrice = table.Column<double>(type: "float", nullable: false),
                    TonnagePrice = table.Column<double>(type: "float", nullable: true),
                    DriverTonnagePrice = table.Column<double>(type: "float", nullable: true),
                    ShippingFeeLoadTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFeeGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingFeeGroup_Contract_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "dbo",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingFeeGroup_ShippingFeeLoadType_ShippingFeeLoadTypeId",
                        column: x => x.ShippingFeeLoadTypeId,
                        principalSchema: "dbo",
                        principalTable: "ShippingFeeLoadType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingFeeRoute",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    OriginId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationId = table.Column<long>(type: "bigint", nullable: false),
                    ShippingFeeGroupId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFeeRoute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingFeeRoute_LoadRoute_DestinationId",
                        column: x => x.DestinationId,
                        principalSchema: "dbo",
                        principalTable: "LoadRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShippingFeeRoute_LoadRoute_OriginId",
                        column: x => x.OriginId,
                        principalSchema: "dbo",
                        principalTable: "LoadRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShippingFeeRoute_ShippingFeeGroup_ShippingFeeGroupId",
                        column: x => x.ShippingFeeGroupId,
                        principalSchema: "dbo",
                        principalTable: "ShippingFeeGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFeeGroup_ContractId",
                schema: "dbo",
                table: "ShippingFeeGroup",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFeeGroup_RowId",
                schema: "dbo",
                table: "ShippingFeeGroup",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFeeGroup_ShippingFeeLoadTypeId",
                schema: "dbo",
                table: "ShippingFeeGroup",
                column: "ShippingFeeLoadTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFeeRoute_DestinationId",
                schema: "dbo",
                table: "ShippingFeeRoute",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFeeRoute_OriginId",
                schema: "dbo",
                table: "ShippingFeeRoute",
                column: "OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFeeRoute_RowId",
                schema: "dbo",
                table: "ShippingFeeRoute",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFeeRoute_ShippingFeeGroupId",
                schema: "dbo",
                table: "ShippingFeeRoute",
                column: "ShippingFeeGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingFeeRoute",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShippingFeeGroup",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "ShippingFeeRouteId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                schema: "dbo",
                table: "LoadFactor");
        }
    }
}
