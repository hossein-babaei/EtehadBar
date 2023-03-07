using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class HasAddonMessage_Mehrcom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAddonMessage",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAddonMessage",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");
        }
    }
}
