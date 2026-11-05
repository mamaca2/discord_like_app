using DiscordApp.Domain.Enums;

namespace DiscordApp.Application.DTOs.FriendRequestDTO;

public class FriendRequestDto
{
    public int Id { get; set; }

    public string SenderId { get; set; } = string.Empty;
    public string SenderUserName { get; set; } = string.Empty;
    public string SenderTag { get; set; } = string.Empty;
    public string? SenderImage { get; set; }

    public string ReceiverId { get; set; } = string.Empty;
    public string ReceiverUserName { get; set; } = string.Empty;
    public string ReceiverTag { get; set; } = string.Empty;
    public string? ReceiverImage { get; set; }

    public DateTime SentAt { get; set; }
    public FriendRequestStatus Status { get; set; }
}