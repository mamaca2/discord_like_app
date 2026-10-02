using DiscordApp.Application.Results;
using DiscordApp.Domain.Common;
using DiscordApp.Domain.constants;

namespace DiscordApp.Application.Services.UserSearchServices;

public record ParsedUserSearchQuery(
    bool HasTag,
    string Username,
    string Tag
);

public static class UserSearchQueryValidator
{
    public static Result<ParsedUserSearchQuery> ParseAndValidate(string? rawQuery)
    {
        if (string.IsNullOrWhiteSpace(rawQuery))
        {
            return Result<ParsedUserSearchQuery>.Success(
                new ParsedUserSearchQuery(HasTag: false, Username: string.Empty, Tag: string.Empty));
        }

        var trimmed = rawQuery.Trim();
        int hashCount = trimmed.Count(c => c == '#');

        if (hashCount > 1)
        {
            return Result<ParsedUserSearchQuery>.BadRequest(
                new Error(
                    ErrorCodes.BadRequest,
                    "Invalid search query. Query can contain at most one '#' character."));
        }

        if (hashCount == 1)
        {
            var parts = trimmed.Split('#', 2);
            return Result<ParsedUserSearchQuery>.Success(
                new ParsedUserSearchQuery(
                    HasTag: true,
                    Username: parts[0],
                    Tag: parts[1]));
        }

        return Result<ParsedUserSearchQuery>.Success(
            new ParsedUserSearchQuery(
                HasTag: false,
                Username: trimmed,
                Tag: string.Empty));
    }
}