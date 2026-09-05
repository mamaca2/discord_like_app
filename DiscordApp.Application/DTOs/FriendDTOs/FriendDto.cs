namespace DiscordApp.Application.DTOs.FriendDTOs;

public class FriendDto { 
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Status { get; set; } = "Offline"; 
}