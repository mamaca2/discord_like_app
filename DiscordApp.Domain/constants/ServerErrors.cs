
using DiscordApp.Domain.Common;

namespace DiscordApp.Domain.Constants;

public static class ServerErrors
{
    public static readonly Error ServerNotFound = new(
        "Server.NotFound",
        "The requested server was not found.");

    public static readonly Error NotMember = new(
        "Server.NotMember",
        "You must be a member of this server to perform this action.");

    public static readonly Error InvalidServerName = new(
        "Server.InvalidName",
        "Server name cannot be empty.");

    public static readonly Error ImageTooLarge = new(
        "Server.ImageTooLarge",
        "Image cannot exceed 5 MB.");

    public static readonly Error UnsupportedImageFormat = new(
        "Server.UnsupportedImageFormat",
        "Unsupported image format.");

    public static readonly Error DeleteServerNotOwner = new(
        "Server.DeleteNotOwner",
        "Only the server owner can delete this server.");

    public static readonly Error CreateChatNotOwner = new(
        "Server.ChatCreationNotOwner",
        "Only the server owner can create chats.");

    public static readonly Error InvalidPagination = new(
        "ServerChat.InvalidPagination",
        "Invalid page or page size.");

    public static readonly Error ChatNotInServer = new(
        "ServerChat.NotInServer",
        "Chat is not in that server.");
}
