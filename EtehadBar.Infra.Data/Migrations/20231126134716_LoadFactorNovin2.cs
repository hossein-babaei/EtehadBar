using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class LoadFactorNovin2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                schema: "dbo",
                table: "LoadFactorNovin",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiveDate",
                schema: "dbo",
                table: "LoadFactorNovin",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                schema: "dbo",
                table: "LoadFactorNovin");

            migrationBuilder.DropColumn(
                name: "ReceiveDate",
                schema: "dbo",
                table: "LoadFactorNovin");
        }
    }
}
