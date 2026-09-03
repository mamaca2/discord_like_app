using System.ComponentModel.DataAnnotations;

public class RegisterUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User";
}