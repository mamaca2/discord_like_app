namespace DiscordApp.Application.DTOs.ServerDTOs;

public class ServerMemberDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string Role { get; set; } = string.Empty;
}