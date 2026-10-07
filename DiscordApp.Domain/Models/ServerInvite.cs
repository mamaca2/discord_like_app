using DiscordApp.Domain.Models;

namespace DiscordApp.Domain.Models;

public class ServerInvite
{
    public string Code { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty; 
    public ServerInfo? Server { get; set; }

    public string CreatorId { get; set; } = string.Empty;
    public User? Creator { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int MaxUses { get; set; }
    public int Uses { get; set; }
}