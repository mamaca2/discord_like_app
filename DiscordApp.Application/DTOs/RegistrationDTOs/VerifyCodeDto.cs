namespace DiscordApp.Application.DTOs.RegistrationDTOs;

public class VerifyCodeDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}