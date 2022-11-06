using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitPoster.DAL.Migrations
{
    public partial class User_Is_Banned : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "UserAccount",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "UserAccount");
        }
    }
}
