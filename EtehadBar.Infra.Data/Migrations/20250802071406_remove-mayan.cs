using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class removemayan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 5L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Customer",
                columns: new[] { "Id", "ActiveBank", "CustomerType", "HasAddonTonnage", "HasLoadSleep", "HasLoadType", "LoadFactorDeductions", "Name", "RowId", "Status" },
                values: new object[] { 5L, 43L, 4, false, false, true, 8.0, "مایان", "8802c985-c29b-4f52-aef9-38dcd4f82eb7", true });
        }
    }
}
