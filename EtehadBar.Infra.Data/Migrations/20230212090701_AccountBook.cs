using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class AccountBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountBookId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "AccountBook",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FactorNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBook", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountBook_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_AccountBookId",
                schema: "dbo",
                table: "LoadFactor",
                column: "AccountBookId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBook_CustomerId",
                schema: "dbo",
                table: "AccountBook",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBook_RowId",
                schema: "dbo",
                table: "AccountBook",
                column: "RowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_AccountBook_AccountBookId",
                schema: "dbo",
                table: "LoadFactor",
                column: "AccountBookId",
                principalSchema: "dbo",
                principalTable: "AccountBook",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_AccountBook_AccountBookId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropTable(
                name: "AccountBook",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_AccountBookId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "AccountBookId",
                schema: "dbo",
                table: "LoadFactor");
        }
    }
}
