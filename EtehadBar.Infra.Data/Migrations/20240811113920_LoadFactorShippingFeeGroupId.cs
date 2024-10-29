using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class LoadFactorShippingFeeGroupId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ShippingFeeId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "ShippingFeeGroupId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingFeeGroupId",
                schema: "dbo",
                table: "LoadFactor");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingFeeId",
                schema: "dbo",
                table: "LoadFactor",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
