using Microsoft.AspNetCore.Identity;

namespace DiscordApp.Server.Models
{
    public class User : IdentityUser
    {
        public string? Image {  get; set; } = string.Empty;

        public string Tag {  get; set; } = string.Empty;

        public IList<User> friend { get; set; } = [];
    }
}