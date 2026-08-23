using DiscordApp.Models;

namespace DiscordApp.DTOs.FriendRequestDTO;

public class FriendRequestDto
{
    public int Id { get; set; }

    public string SenderId { get; set; } = string.Empty;
    public string SenderUserName { get; set; } = string.Empty;

    public string ReceiverId { get; set; } = string.Empty;
    public string ReceiverUserName { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }
    public FriendRequestStatus Status { get; set; }
}