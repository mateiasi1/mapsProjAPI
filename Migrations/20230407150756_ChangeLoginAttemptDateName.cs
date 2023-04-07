using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mapsProjAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLoginAttemptDateName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidUntill",
                table: "LoginAttempts",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "AttemptCounter",
                table: "LoginAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptCounter",
                table: "LoginAttempts");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "LoginAttempts",
                newName: "ValidUntill");
        }
    }
}
