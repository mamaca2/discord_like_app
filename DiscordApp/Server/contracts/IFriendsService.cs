// contracts/IFriendsService.cs
using DiscordApp.DTOs.FriendDTOs;
using DiscordApp.Server.Results;

namespace DiscordApp.contracts;

public interface IFriendsService
{
    Task<Result<List<FriendDto>>> GetFriendsListAsync(string userId);
    Task<Result> UnfriendAsync(string userId, string friendId);
}