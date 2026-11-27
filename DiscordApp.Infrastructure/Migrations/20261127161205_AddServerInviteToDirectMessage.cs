using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServerInviteToDirectMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServerInviteId",
                table: "DirectMessages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_ServerInviteId",
                table: "DirectMessages",
                column: "ServerInviteId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessages_ServerInvites_ServerInviteId",
                table: "DirectMessages",
                column: "ServerInviteId",
                principalTable: "ServerInvites",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessages_ServerInvites_ServerInviteId",
                table: "DirectMessages");

            migrationBuilder.DropIndex(
                name: "IX_DirectMessages_ServerInviteId",
                table: "DirectMessages");

            migrationBuilder.DropColumn(
                name: "ServerInviteId",
                table: "DirectMessages");
        }
    }
}
