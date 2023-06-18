using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class CustomerFactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBook_Contract_ContractId",
                schema: "dbo",
                table: "AccountBook");

            migrationBuilder.DropIndex(
                name: "IX_AccountBook_ContractId",
                schema: "dbo",
                table: "AccountBook");

            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "dbo",
                table: "AccountBook");

            migrationBuilder.CreateTable(
                name: "CustomerFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FactorNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerFactor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerFactor_Contract_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "dbo",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFactor_ContractId",
                schema: "dbo",
                table: "CustomerFactor",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFactor_RowId",
                schema: "dbo",
                table: "CustomerFactor",
                column: "RowId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerFactor",
                schema: "dbo");

            migrationBuilder.AddColumn<long>(
                name: "ContractId",
                schema: "dbo",
                table: "AccountBook",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_AccountBook_ContractId",
                schema: "dbo",
                table: "AccountBook",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBook_Contract_ContractId",
                schema: "dbo",
                table: "AccountBook",
                column: "ContractId",
                principalSchema: "dbo",
                principalTable: "Contract",
                principalColumn: "Id");
        }
    }
}
