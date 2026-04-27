using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MFO.UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdtoUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Email_Active",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email_Active",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[IsActive] = 1");
        }
    }
}
