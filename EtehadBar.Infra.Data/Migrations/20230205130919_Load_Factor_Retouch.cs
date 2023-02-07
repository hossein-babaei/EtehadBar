using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class Load_Factor_Retouch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Sequence",
                schema: "dbo",
                table: "SazehGostarLoadFactor",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Sequence",
                schema: "dbo",
                table: "SaipaPressLoadFactor",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDateTime",
                schema: "dbo",
                table: "LoadFactor",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EditDateTime",
                schema: "dbo",
                table: "LoadFactor",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditorId",
                schema: "dbo",
                table: "LoadFactor",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SaipaPlascoLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MehrcomParsLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sequence = table.Column<long>(type: "bigint", nullable: false),
                    LoadFactorId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MehrcomParsLoadFactor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaipaPlascoLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sequence = table.Column<long>(type: "bigint", nullable: false),
                    LoadFactorId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaipaPlascoLoadFactor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SazehGostarLoadFactor_Sequence",
                schema: "dbo",
                table: "SazehGostarLoadFactor",
                column: "Sequence",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaipaPressLoadFactor_Sequence",
                schema: "dbo",
                table: "SaipaPressLoadFactor",
                column: "Sequence",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "MehrcomParsLoadFactorId",
                unique: true,
                filter: "[MehrcomParsLoadFactorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_SaipaPlascoLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SaipaPlascoLoadFactorId",
                unique: true,
                filter: "[SaipaPlascoLoadFactorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MehrcomParsLoadFactor_RowId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MehrcomParsLoadFactor_Sequence",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                column: "Sequence",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaipaPlascoLoadFactor_RowId",
                schema: "dbo",
                table: "SaipaPlascoLoadFactor",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaipaPlascoLoadFactor_Sequence",
                schema: "dbo",
                table: "SaipaPlascoLoadFactor",
                column: "Sequence",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_MehrcomParsLoadFactor_MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "MehrcomParsLoadFactorId",
                principalSchema: "dbo",
                principalTable: "MehrcomParsLoadFactor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_SaipaPlascoLoadFactor_SaipaPlascoLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SaipaPlascoLoadFactorId",
                principalSchema: "dbo",
                principalTable: "SaipaPlascoLoadFactor",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_MehrcomParsLoadFactor_MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_SaipaPlascoLoadFactor_SaipaPlascoLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropTable(
                name: "MehrcomParsLoadFactor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SaipaPlascoLoadFactor",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_SazehGostarLoadFactor_Sequence",
                schema: "dbo",
                table: "SazehGostarLoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_SaipaPressLoadFactor_Sequence",
                schema: "dbo",
                table: "SaipaPressLoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_SaipaPlascoLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "Sequence",
                schema: "dbo",
                table: "SazehGostarLoadFactor");

            migrationBuilder.DropColumn(
                name: "Sequence",
                schema: "dbo",
                table: "SaipaPressLoadFactor");

            migrationBuilder.DropColumn(
                name: "CreateDateTime",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "EditDateTime",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "EditorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "SaipaPlascoLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");
        }
    }
}
