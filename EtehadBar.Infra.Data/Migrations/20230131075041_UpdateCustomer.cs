using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class UpdateCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAddonTonnage",
                schema: "dbo",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasLoadType",
                schema: "dbo",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "HasAddonTonnage", "HasLoadType" },
                values: new object[] { true, true });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Customer",
                columns: new[] { "Id", "CustomerType", "HasAddonTonnage", "HasLoadType", "Name", "RowId", "Status" },
                values: new object[] { 4L, 3, false, false, "مهرکام پارس", "e70bffab-fa42-4c66-8af8-d7090a6ccbea", true });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "ShippingFeeLoadType",
                columns: new[] { "Id", "Name", "RowId" },
                values: new object[] { -1L, "کالا", "e015d881-cf4f-40b2-bf83-0a115bae3179" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "ShippingFeeLoadType",
                keyColumn: "Id",
                keyValue: -1L);

            migrationBuilder.DropColumn(
                name: "HasAddonTonnage",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "HasLoadType",
                schema: "dbo",
                table: "Customer");
        }
    }
}
