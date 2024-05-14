using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class LoadFactorGovRegistor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LoadFactorGovAmount",
                schema: "dbo",
                table: "LoadFactor",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LoadFactorGovRegistorId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_LoadFactorGovRegistorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "LoadFactorGovRegistorId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_Definition_LoadFactorGovRegistorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "LoadFactorGovRegistorId",
                principalSchema: "dbo",
                principalTable: "Definition",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_Definition_LoadFactorGovRegistorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_LoadFactorGovRegistorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadFactorGovAmount",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadFactorGovRegistorId",
                schema: "dbo",
                table: "LoadFactor");
        }
    }
}
