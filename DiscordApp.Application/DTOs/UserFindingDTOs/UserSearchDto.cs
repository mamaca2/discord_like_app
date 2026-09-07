namespace DiscordApp.Application.DTOs.UserFindingDTOs;

public class UserSearchDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string? Image { get; set; }
}