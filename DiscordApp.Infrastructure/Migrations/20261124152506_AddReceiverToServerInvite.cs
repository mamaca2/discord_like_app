using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiverToServerInvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "ServerInvites",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "ServerInvites");
        }
    }
}
