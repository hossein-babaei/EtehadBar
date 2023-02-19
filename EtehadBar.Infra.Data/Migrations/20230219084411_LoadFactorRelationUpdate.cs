using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class LoadFactorRelationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_MehrcomParsLoadFactor_MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_SaipaPlascoLoadFactor_SaipaPlascoLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_SaipaPressLoadFactor_SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_SazehGostarLoadFactor_SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_SaipaPlascoLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_LoadFactor_SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "ShippingFee");

            migrationBuilder.DropColumn(
                name: "LoadSleepPrice",
                schema: "dbo",
                table: "ShippingFee");

            migrationBuilder.DropColumn(
                name: "MehrcomParsLoadFactorId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropColumn(
                name: "SaipaPlascoLoadFactorId",
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

            migrationBuilder.CreateIndex(
                name: "IX_SazehGostarLoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SazehGostarLoadFactor",
                column: "LoadFactorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaipaPressLoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SaipaPressLoadFactor",
                column: "LoadFactorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaipaPlascoLoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SaipaPlascoLoadFactor",
                column: "LoadFactorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MehrcomParsLoadFactor_LoadFactorId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                column: "LoadFactorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MehrcomParsLoadFactor_LoadFactor_LoadFactorId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor",
                column: "LoadFactorId",
                principalSchema: "dbo",
                principalTable: "LoadFactor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaipaPlascoLoadFactor_LoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SaipaPlascoLoadFactor",
                column: "LoadFactorId",
                principalSchema: "dbo",
                principalTable: "LoadFactor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaipaPressLoadFactor_LoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SaipaPressLoadFactor",
                column: "LoadFactorId",
                principalSchema: "dbo",
                principalTable: "LoadFactor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SazehGostarLoadFactor_LoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SazehGostarLoadFactor",
                column: "LoadFactorId",
                principalSchema: "dbo",
                principalTable: "LoadFactor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MehrcomParsLoadFactor_LoadFactor_LoadFactorId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.DropForeignKey(
                name: "FK_SaipaPlascoLoadFactor_LoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SaipaPlascoLoadFactor");

            migrationBuilder.DropForeignKey(
                name: "FK_SaipaPressLoadFactor_LoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SaipaPressLoadFactor");

            migrationBuilder.DropForeignKey(
                name: "FK_SazehGostarLoadFactor_LoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SazehGostarLoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_SazehGostarLoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SazehGostarLoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_SaipaPressLoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SaipaPressLoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_SaipaPlascoLoadFactor_LoadFactorId",
                schema: "dbo",
                table: "SaipaPlascoLoadFactor");

            migrationBuilder.DropIndex(
                name: "IX_MehrcomParsLoadFactor_LoadFactorId",
                schema: "dbo",
                table: "MehrcomParsLoadFactor");

            migrationBuilder.AddColumn<double>(
                name: "DriverLoadSleepPrice",
                schema: "dbo",
                table: "ShippingFee",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LoadSleepPrice",
                schema: "dbo",
                table: "ShippingFee",
                type: "float",
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

            migrationBuilder.AddColumn<long>(
                name: "SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_SaipaPressLoadFactor_SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SaipaPressLoadFactorId",
                principalSchema: "dbo",
                principalTable: "SaipaPressLoadFactor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_SazehGostarLoadFactor_SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SazehGostarLoadFactorId",
                principalSchema: "dbo",
                principalTable: "SazehGostarLoadFactor",
                principalColumn: "Id");
        }
    }
}
