using Microsoft.EntityFrameworkCore.Migrations;

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class ShippingFeeIdInLoadFactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingFeeId",
                schema: "dbo",
                table: "LoadFactor",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingFeeId",
                schema: "dbo",
                table: "LoadFactor");
        }
    }
}
