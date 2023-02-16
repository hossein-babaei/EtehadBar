using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class Driver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_AspNetUsers_DriverId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.AlterColumn<long>(
                name: "DriverId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.CreateTable(
                name: "Driver",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AccountBankName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Phonenumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    NationalNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Driver", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Driver_RowId",
                schema: "dbo",
                table: "Driver",
                column: "RowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_Driver_DriverId",
                schema: "dbo",
                table: "LoadFactor",
                column: "DriverId",
                principalSchema: "dbo",
                principalTable: "Driver",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadFactor_Driver_DriverId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.DropTable(
                name: "Driver",
                schema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                schema: "dbo",
                table: "LoadFactor",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_LoadFactor_AspNetUsers_DriverId",
                schema: "dbo",
                table: "LoadFactor",
                column: "DriverId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
