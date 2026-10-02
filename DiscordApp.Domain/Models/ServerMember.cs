// Domain/Models/ServerMember.cs
using DiscordApp.Domain.Enums;

namespace DiscordApp.Domain.Models;

public class ServerMember
{
    public int Id { get; set; }

    public int ServerInfoId { get; set; }
    public ServerInfo Server { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public ServerRole Role { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}