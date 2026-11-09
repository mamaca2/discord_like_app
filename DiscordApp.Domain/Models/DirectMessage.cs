namespace DiscordApp.Domain.Models;

public class DirectMessage
{
    public int Id { get; set; }

    public string SenderId { get; set; } = string.Empty;
    public User? Sender { get; set; }

    public string ReceiverId { get; set; } = string.Empty;
    public User? Receiver { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}