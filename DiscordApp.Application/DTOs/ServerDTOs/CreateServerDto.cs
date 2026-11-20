using Microsoft.AspNetCore.Http;

namespace DiscordApp.Application.DTOs.ServerDTOs;

public class CreateServerDto
{
    public string Name { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}