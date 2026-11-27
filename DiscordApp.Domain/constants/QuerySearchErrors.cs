using DiscordApp.Domain.Common;

namespace DiscordApp.Domain.Constants;

public static class UserSearchErrors
{
    public static readonly Error InvalidQuery = new(
        "UserSearch.InvalidQuery",
        "Invalid search query. Query can contain at most one '#' character.");
}
