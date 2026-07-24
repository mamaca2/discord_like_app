using DiscordApp.Server.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    //public DbSet<Server> Servers { get; set; }

    //public DbSet<Channel> Channels { get; set; }

    //public DbSet<Message> Messages { get; set; }

    //public DbSet<ServerMember> ServerMembers { get; set; }
}