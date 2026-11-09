using System;

namespace DiscordApp.Application.DTOs.ChatDTOs;

public class DirectMessageDTO
{
    public string ReceiverId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
