using Microsoft.EntityFrameworkCore.Migrations;

namespace alcobot.service.Migrations
{
    public partial class MovedChatId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Drinkers");

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "Drinks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Drinks");

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "Drinkers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
