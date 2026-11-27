namespace DiscordApp.Application.DTOs.ServerDTOs.ChatDTOs;

public class ServerChatDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ServerId { get; set; }
}