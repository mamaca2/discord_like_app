namespace DiscordApp.Application.DTOs.ServerDTOs;

public class CreateServerDto
{
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
}