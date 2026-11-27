using DiscordApp.Domain.Common;

namespace DiscordApp.Domain.Constants;

public static class FriendsErrors
{
    public static readonly Error FriendshipNotFound = new(
        "Friends.NotFound",
        "Friendship relationship not found.");
}
