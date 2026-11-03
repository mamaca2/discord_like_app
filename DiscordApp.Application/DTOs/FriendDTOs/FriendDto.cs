using DiscordApp.Domain.Enums;

namespace DiscordApp.Application.DTOs.FriendDTOs;

public class FriendDto { 
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.Offline; 
}