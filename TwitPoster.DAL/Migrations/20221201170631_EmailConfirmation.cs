using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitPoster.DAL.Migrations
{
    public partial class EmailConfirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmailConfirmationToken",
                table: "UserAccount",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "UserAccount",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "UserAccount");

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "UserAccount");
        }
    }
}
