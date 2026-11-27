namespace DiscordApp.Application.DTOs.ServerDTOs.ChatDTOs;

public class ServerChatMessageDto
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsMine { get; set; }
}