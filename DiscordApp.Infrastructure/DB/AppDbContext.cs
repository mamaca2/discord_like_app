using DiscordApp.Application.Interfaces;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Infrastructure.DB;

public class AppDbContext : IdentityDbContext<User>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<FriendRequest> FriendRequests { get; set; }
    public DbSet<PendingRegistration> PendingRegistrations { get; set; } = null!;
    public DbSet<ServerInfo> ServerInfos => throw new NotImplementedException();
    public DbSet<ServerMember> ServerMembers {get; set;}


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasIndex(u => new { u.UserName, u.Tag })
            .IsUnique();
    }
}