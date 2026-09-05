using DiscordApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordApp.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<FriendRequest> FriendRequests { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}