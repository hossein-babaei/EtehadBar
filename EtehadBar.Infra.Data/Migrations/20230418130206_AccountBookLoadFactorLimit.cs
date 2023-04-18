using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class AccountBookLoadFactorLimit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoadFactorLimit",
                schema: "dbo",
                table: "AccountBook",
                type: "int",
                nullable: false,
                defaultValue: 150);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadFactorLimit",
                schema: "dbo",
                table: "AccountBook");
        }
    }
}
