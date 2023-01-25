using Microsoft.EntityFrameworkCore.Migrations;

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class FixEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "dbo",
                table: "Definition",
                newName: "DefinitionType");

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                schema: "dbo",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerType",
                schema: "dbo",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Customer",
                columns: new[] { "Id", "CustomerType", "Name", "Status" },
                values: new object[] { 1, 0, "پلاسکو کار سایپا", true });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Customer",
                columns: new[] { "Id", "CustomerType", "Name", "Status" },
                values: new object[] { 2, 1, "سایپا پرس", true });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Customer",
                columns: new[] { "Id", "CustomerType", "Name", "Status" },
                values: new object[] { 3, 2, "سازه گستر", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "PaymentType",
                schema: "dbo",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "DefinitionType",
                schema: "dbo",
                table: "Definition",
                newName: "Type");

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                schema: "dbo",
                table: "Payment",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                schema: "dbo",
                table: "Customer",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
