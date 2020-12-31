using Microsoft.EntityFrameworkCore.Migrations;

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Type",
                schema: "dbo",
                table: "Customer",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "Customer");
        }
    }
}
