namespace DiscordApp.Domain.Common;

public readonly record struct Error(string Code, string Description)
{
    public static readonly Error None = new("", "");
    public bool IsNone => string.IsNullOrWhiteSpace(Code);
}