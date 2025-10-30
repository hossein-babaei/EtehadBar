using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class BillDetail_Turnover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ProfitPercent",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountOwner",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TurnoverPaymentType",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TurnoverTurnType",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                schema: "dbo",
                table: "Bill",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BillDetail",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiverName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ReceiverBankAccount = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    BillId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillDetail_Bill_BillId",
                        column: x => x.BillId,
                        principalSchema: "dbo",
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 4L,
                column: "LoadFactorDeductions",
                value: 7.8);

            migrationBuilder.CreateIndex(
                name: "IX_TurnoverProfile_CustomerId",
                schema: "dbo",
                table: "TurnoverProfile",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BillDetail_BillId",
                schema: "dbo",
                table: "BillDetail",
                column: "BillId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillDetail_RowId",
                schema: "dbo",
                table: "BillDetail",
                column: "RowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TurnoverProfile_Customer_CustomerId",
                schema: "dbo",
                table: "TurnoverProfile",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TurnoverProfile_Customer_CustomerId",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropTable(
                name: "BillDetail",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_TurnoverProfile_CustomerId",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "BankAccountOwner",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "TurnoverPaymentType",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "TurnoverTurnType",
                schema: "dbo",
                table: "TurnoverProfile");

            migrationBuilder.DropColumn(
                name: "IsReturned",
                schema: "dbo",
                table: "Bill");

            migrationBuilder.AlterColumn<int>(
                name: "ProfitPercent",
                schema: "dbo",
                table: "TurnoverProfile",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 4L,
                column: "LoadFactorDeductions",
                value: 5.0);
        }
    }
}
