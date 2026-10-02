using DiscordApp.Domain.Enums;

namespace DiscordApp.Domain.Models;

public class ServerMember
{
    public string ServerId { get; set; } = string.Empty;
    public Server Server { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public ServerRole Role { get; set; } = ServerRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}