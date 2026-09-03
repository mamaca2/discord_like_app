using DiscordApp.Server.Models;

namespace DiscordApp.Models;

public class FriendRequest
{
    public int Id { get; set; }

    public string SenderId { get; set; } = string.Empty;
    public User? Sender { get; set; }

    public string ReceiverId { get; set; } = string.Empty;
    public User? Receiver { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
}

public enum FriendRequestStatus
{
    Pending,
    Accepted
}