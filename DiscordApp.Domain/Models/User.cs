using Microsoft.AspNetCore.Identity;

namespace DiscordApp.Domain.Models;

public class User : IdentityUser
{
    public string? Image { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
}