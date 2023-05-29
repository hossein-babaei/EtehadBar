using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class VehicleUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDatetime",
                schema: "dbo",
                table: "Vehicles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditDatetime",
                schema: "dbo",
                table: "Vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditorId",
                schema: "dbo",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDatetime",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "EditDatetime",
                schema: "dbo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "EditorId",
                schema: "dbo",
                table: "Vehicles");
        }
    }
}
