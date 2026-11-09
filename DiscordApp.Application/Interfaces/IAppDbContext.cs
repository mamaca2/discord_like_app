using DiscordApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }

    DbSet<FriendRequest> FriendRequests { get; }
    DbSet<PendingRegistration> PendingRegistrations { get; }

    DbSet<ServerInfo> ServerInfos { get; }
    DbSet<ServerMember> ServerMembers { get; }
    DbSet<ServerInvite> ServerInvites { get; }

    DbSet<DirectMessage> DirectMessages { get; }

    DbSet<ServerChat> ServerChats { get; }
    DbSet<ChatMessage> ChatMessages { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}