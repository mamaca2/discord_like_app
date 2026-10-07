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
    public DbSet<ServerInfo> ServerInfos { get; set; }
    public DbSet<ServerMember> ServerMembers {get; set;}
    public DbSet<ServerInvite> ServerInvites {get; set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ServerMember>()
            .HasKey(sm => new { sm.ServerInfoId, sm.UserId });

        builder.Entity<ServerMember>()
            .HasOne(sm => sm.Server)
            .WithMany(s => s.Members)
            .HasForeignKey(sm => sm.ServerInfoId);
    }
}