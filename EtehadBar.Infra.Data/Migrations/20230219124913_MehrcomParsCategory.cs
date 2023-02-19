using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class MehrcomParsCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "LoadType",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MehrcomParsCategory",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MehrcomParsCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MehrcomParsLoadFactor_CategoryId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MehrcomParsCategory_RowId",
                schema: "dbo",
                table: "MehrcomParsCategory",
                column: "RowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MehrcomParsLoadFactor_MehrcomParsCategory_CategoryId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                column: "CategoryId",
                principalSchema: "dbo",
                principalTable: "MehrcomParsCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MehrcomParsLoadFactor_MehrcomParsCategory_CategoryId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropTable(
                name: "MehrcomParsCategory",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_MehrcomParsLoadFactor_CategoryId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropColumn(
                name: "LoadType",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");
        }
    }
}
