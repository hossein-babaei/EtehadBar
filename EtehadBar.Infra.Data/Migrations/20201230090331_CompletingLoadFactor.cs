using Microsoft.EntityFrameworkCore.Migrations;

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CompletingLoadFactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaipaPressLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoadType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LoadFactorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaipaPressLoadFactor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SazehGostarLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegisterCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Certain = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Nature = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false),
                    DetailedCostCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoadFactorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SazehGostarLoadFactor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SaipaPressLoadFactorId",
                unique: true,
                filter: "[SaipaPressLoadFactorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SazehGostarLoadFactorId",
                unique: true,
                filter: "[SazehGostarLoadFactorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_SaipaPressLoadFactor_SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SaipaPressLoadFactorId",
                principalSchema: "dbo",
                principalTable: "SaipaPressLoadFactor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_SazehGostarLoadFactor_SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SazehGostarLoadFactorId",
                principalSchema: "dbo",
                principalTable: "SazehGostarLoadFactor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_SaipaPressLoadFactor_SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_SazehGostarLoadFactor_SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropTable(
                name: "SaipaPressLoadFactor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SazehGostarLoadFactor",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");
        }
    }
}
