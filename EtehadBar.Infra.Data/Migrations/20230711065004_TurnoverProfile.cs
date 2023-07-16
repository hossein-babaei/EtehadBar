using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class TurnoverProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TurnoverProfileId",
                schema: "dbo",
                table: "Turnover",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TurnoverProfile",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TurnoverType = table.Column<int>(type: "int", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnoverProfile", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turnover_TurnoverProfileId",
                schema: "dbo",
                table: "Turnover",
                column: "TurnoverProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoverProfile_RowId",
                schema: "dbo",
                table: "TurnoverProfile",
                column: "RowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnover_TurnoverProfile_TurnoverProfileId",
                schema: "dbo",
                table: "Turnover",
                column: "TurnoverProfileId",
                principalSchema: "dbo",
                principalTable: "TurnoverProfile",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnover_TurnoverProfile_TurnoverProfileId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.DropTable(
                name: "TurnoverProfile",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Turnover_TurnoverProfileId",
                schema: "dbo",
                table: "Turnover");

            migrationBuilder.DropColumn(
                name: "TurnoverProfileId",
                schema: "dbo",
                table: "Turnover");
        }
    }
}
