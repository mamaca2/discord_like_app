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
                UserSearchErrors.InvalidQuery);
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
