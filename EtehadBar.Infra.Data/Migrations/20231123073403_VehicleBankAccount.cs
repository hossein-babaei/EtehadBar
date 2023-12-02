using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class VehicleBankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ActiveBank",
                schema: "dbo",
                table: "Customer",
                type: "bigint",
                nullable: false,
                defaultValue: 43L);

            migrationBuilder.CreateTable(
                name: "VehicleBankAccounts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleBankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleBankAccounts_Definition_BankId",
                        column: x => x.BankId,
                        principalSchema: "dbo",
                        principalTable: "Definition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleBankAccounts_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "dbo",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_ActiveBank",
                schema: "dbo",
                table: "Customer",
                column: "ActiveBank");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBankAccounts_BankId",
                schema: "dbo",
                table: "VehicleBankAccounts",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBankAccounts_RowId",
                schema: "dbo",
                table: "VehicleBankAccounts",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBankAccounts_VehicleId",
                schema: "dbo",
                table: "VehicleBankAccounts",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Definition_ActiveBank",
                schema: "dbo",
                table: "Customer",
                column: "ActiveBank",
                principalSchema: "dbo",
                principalTable: "Definition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Definition_ActiveBank",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.DropTable(
                name: "VehicleBankAccounts",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Customer_ActiveBank",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "ActiveBank",
                schema: "dbo",
                table: "Customer");
        }
    }
}
