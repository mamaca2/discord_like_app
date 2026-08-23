using System.ComponentModel.DataAnnotations;

namespace DiscordApp.DTOs;

public class RegisterUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string username { get; set; } = string.Empty;

    [Required, MaxLength(100)]

    public string Role { get; set; } = "User";
}
