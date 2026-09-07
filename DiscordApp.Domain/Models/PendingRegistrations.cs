namespace DiscordApp.Domain.Models;

public class PendingRegistration
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public DateTime ExpiresAt { get; set; }
}