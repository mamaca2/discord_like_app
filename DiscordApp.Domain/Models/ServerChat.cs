namespace DiscordApp.Domain.Models;

public class ServerChat
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int ServerId { get; set; }
    public ServerInfo Server { get; set; } = null!;

    public ICollection<ChatMessage> ChatMessages { get; set; } = [];
}