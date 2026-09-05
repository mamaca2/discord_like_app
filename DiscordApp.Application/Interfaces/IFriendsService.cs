using DiscordApp.Application.DTOs.FriendDTOs;
using DiscordApp.Application.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordApp.Application.contracts;

public interface IFriendsService
{
    Task<Result<List<FriendDto>>> GetFriendsListAsync(string userId);
    Task<Result> UnfriendAsync(string userId, string friendId);
}