using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordApp.Migrations
{
    /// <inheritdoc />
    public partial class SyncUserAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "code",
                table: "AspNetUsers",
                newName: "Tag");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName_Tag",
                table: "AspNetUsers",
                columns: new[] { "UserName", "Tag" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserName_Tag",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Tag",
                table: "AspNetUsers",
                newName: "code");
        }
    }
}
