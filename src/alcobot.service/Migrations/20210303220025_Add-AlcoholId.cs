using Microsoft.EntityFrameworkCore.Migrations;

namespace alcobot.service.Migrations
{
    public partial class AddAlcoholId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AlcoholId",
                table: "Drinks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlcoholId",
                table: "Drinks");
        }
    }
}
