using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitPoster.DAL.Migrations
{
    public partial class AddSeedDataForUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedAt", "Email", "FirstName", "LastName" },
                values: new object[] { 1, new DateTime(1996, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "oleksii.korniienko@twitposter.com", "Oleksii", "Korniienko" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedAt", "Email", "FirstName", "LastName" },
                values: new object[] { 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@twitposter.com", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedAt", "Email", "FirstName", "LastName" },
                values: new object[] { 3, new DateTime(2000, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "moderator@twitposter.com", "Moderator", "Moderator" });

            migrationBuilder.InsertData(
                table: "UserAccount",
                columns: new[] { "Id", "IsBanned", "Password", "Role", "UserId" },
                values: new object[] { 1, false, "Qwerty123", "DatabaseOwner", 1 });

            migrationBuilder.InsertData(
                table: "UserAccount",
                columns: new[] { "Id", "IsBanned", "Password", "Role", "UserId" },
                values: new object[] { 2, false, "Qwerty123", "Admin", 2 });

            migrationBuilder.InsertData(
                table: "UserAccount",
                columns: new[] { "Id", "IsBanned", "Password", "Role", "UserId" },
                values: new object[] { 3, false, "Qwerty123", "Moderator", 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserAccount",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserAccount",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserAccount",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
