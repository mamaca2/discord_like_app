namespace DiscordApp.Domain.Models;

public class ChatMessage
{
    public int Id { get; set; }

    public string SenderId { get; set; } = string.Empty;
    public User? Sender { get; set; }

    public int ServerChatId { get; set; }
    public ServerChat ServerChat { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}