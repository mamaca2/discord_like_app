using System.ComponentModel.DataAnnotations;

namespace DiscordApp.Application.DTOs.AuthenticationDTOs;

public class LoginUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}