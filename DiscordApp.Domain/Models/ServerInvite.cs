using System;

namespace DiscordApp.Domain.Models;

public class ServerInvite
{
    public string Code { get; set; } = Guid.NewGuid().ToString()[..8];
    
    public string ServerId { get; set; } = null!;
    public ServerInfo Server { get; set; } = null!;

    public string CreatorId { get; set; } = null!;
    public User Creator { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    public int MaxUses { get; set; } = 0;
    public int Uses { get; set; } = 0;

    public bool IsExpired => 
        (ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value) || 
        (MaxUses > 0 && Uses >= MaxUses);
}
