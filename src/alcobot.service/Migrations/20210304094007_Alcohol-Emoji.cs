using Microsoft.EntityFrameworkCore.Migrations;

namespace alcobot.service.Migrations
{
    public partial class AlcoholEmoji : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Emoji",
                table: "Alcoholes",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emoji",
                table: "Alcoholes");
        }
    }
}
