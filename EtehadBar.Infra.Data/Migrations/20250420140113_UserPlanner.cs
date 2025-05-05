using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class UserPlanner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPlanner",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlanner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlanner_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPlannerItem",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPlannerId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlannerItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlannerItem_UserPlanner_UserPlannerId",
                        column: x => x.UserPlannerId,
                        principalSchema: "dbo",
                        principalTable: "UserPlanner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "Year",
                value: "1404");

            //migrationBuilder.InsertData(
            //    schema: "dbo",
            //    table: "Customer",
            //    columns: new[] { "Id", "ActiveBank", "CustomerType", "HasAddonTonnage", "HasLoadSleep", "HasLoadType", "LoadFactorDeductions", "Name", "RowId", "Status" },
            //    values: new object[] { 5L, 43L, 4, false, false, true, 8.0, "مایان", "8802c985-c29b-4f52-aef9-38dcd4f82eb7", true });

            migrationBuilder.CreateIndex(
                name: "IX_UserPlanner_RowId",
                schema: "dbo",
                table: "UserPlanner",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlanner_UserId",
                schema: "dbo",
                table: "UserPlanner",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlannerItem_RowId",
                schema: "dbo",
                table: "UserPlannerItem",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlannerItem_UserPlannerId",
                schema: "dbo",
                table: "UserPlannerItem",
                column: "UserPlannerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPlannerItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserPlanner",
                schema: "dbo");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Customer",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "Year",
                value: "1403");
        }
    }
}
