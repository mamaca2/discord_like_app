using DiscordApp.Domain.Common;

namespace DiscordApp.Domain.Constants;

public static class ServerInviteErrors
{
    public static readonly Error InviteCreationFailed = new(
        "Invite.CreationFailed",
        "Failed to create server invite.");

    public static readonly Error InvitationCreatorNotOwner = new(
        "Invite.CreatorNotOwner",
        "Only the server owner can create an invitation.");

    public static readonly Error InvalidInviteCode = new(
        "Invite.InvalidCode",
        "Invalid invite code.");

    public static readonly Error InvitationNotForYou = new(
        "Invite.NotForYou",
        "This invitation is not for you.");

    public static readonly Error InviteExpired = new(
        "Invite.Expired",
        "This invite code has expired.");

    public static readonly Error InviteMaxUsesReached = new(
        "Invite.MaxUsesReached",
        "This invite code has reached its maximum usage limit.");

    public static readonly Error AlreadyMember = new(
        "Invite.AlreadyMember",
        "You are already a member of this server.");
}
