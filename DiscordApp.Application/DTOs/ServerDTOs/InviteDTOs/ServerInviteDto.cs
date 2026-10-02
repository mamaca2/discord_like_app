namespace DiscordApp.Application.DTOs.ServerDTOs;

public class ServerInviteDto
{
    public string Code { get; set; } = null!;
    public string ServerId { get; set; } = null!;
    public string ServerName { get; set; } = null!;
    
    public string CreatorId { get; set; } = null!;
    public string CreatorUsername { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public int MaxUses { get; set; }
    public int Uses { get; set; }

    public bool IsExpired { get; set; }
}