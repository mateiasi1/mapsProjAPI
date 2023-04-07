using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mapsProjAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddValidUntillToLoginAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "LoginAttempts",
                newName: "ValidUntill");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "LoginAttempts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "LoginAttempts");

            migrationBuilder.RenameColumn(
                name: "ValidUntill",
                table: "LoginAttempts",
                newName: "Date");
        }
    }
}
