using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class BankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccount",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountBankName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreateDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccount_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BankAccountBook",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Debtor = table.Column<double>(type: "float", nullable: false),
                    Creditor = table.Column<double>(type: "float", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    BankAccountId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EditDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccountBook", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccountBook_BankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalSchema: "dbo",
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_OwnerUserId",
                schema: "dbo",
                table: "BankAccount",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_RowId",
                schema: "dbo",
                table: "BankAccount",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountBook_BankAccountId",
                schema: "dbo",
                table: "BankAccountBook",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountBook_RowId",
                schema: "dbo",
                table: "BankAccountBook",
                column: "RowId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccountBook",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BankAccount",
                schema: "dbo");
        }
    }
}
