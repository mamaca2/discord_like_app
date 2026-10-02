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

    public static readonly Error InviteCreationFailed = new(
        "Invite.CreationFailed",
        "Failed to create server invite.");
}