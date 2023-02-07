using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CreatorId_EditorId_Date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "ShippingFee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                schema: "dbo",
                table: "ShippingFee",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditDate",
                schema: "dbo",
                table: "ShippingFee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditorId",
                schema: "dbo",
                table: "ShippingFee",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Calendar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                schema: "dbo",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditDate",
                schema: "dbo",
                table: "Calendar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditorId",
                schema: "dbo",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "dbo",
                table: "ShippingFee");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                schema: "dbo",
                table: "ShippingFee");

            migrationBuilder.DropColumn(
                name: "EditDate",
                schema: "dbo",
                table: "ShippingFee");

            migrationBuilder.DropColumn(
                name: "EditorId",
                schema: "dbo",
                table: "ShippingFee");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "dbo",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                schema: "dbo",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "EditDate",
                schema: "dbo",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "EditorId",
                schema: "dbo",
                table: "Calendar");
        }
    }
}
