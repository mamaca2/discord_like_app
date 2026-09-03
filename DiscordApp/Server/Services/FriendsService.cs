using DiscordApp.constants;
using DiscordApp.contracts;
using DiscordApp.DTOs.FriendDTOs;
using DiscordApp.Models;
using DiscordApp.Server.DB;
using DiscordApp.Server.Results;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Services;

public class FriendsService(AppDbContext appDbContext) : IFriendsService
{
    public async Task<Result<List<FriendDto>>> GetFriendsListAsync(
        string userId)
    {
        var friends = await appDbContext.FriendRequests
            .Where(fr =>
                fr.Status == FriendRequestStatus.Accepted &&
                (fr.SenderId == userId ||
                 fr.ReceiverId == userId))
            .Select(fr =>
                fr.SenderId == userId
                    ? fr.Receiver
                    : fr.Sender)
            .Select(user => new FriendDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Tag = user.Tag,
                Image = user.Image ?? string.Empty,
                Status = "Offline"
            })
            .ToListAsync();

        return Result<List<FriendDto>>.Success(friends);
    }

    public async Task<Result> UnfriendAsync(
        string userId,
        string friendId)
    {
        var friendship = await appDbContext.FriendRequests
            .FirstOrDefaultAsync(fr =>
                fr.Status == FriendRequestStatus.Accepted &&
                ((fr.SenderId == userId &&
                  fr.ReceiverId == friendId) ||
                 (fr.SenderId == friendId &&
                  fr.ReceiverId == userId)));

        if (friendship is null)
        {
            return Result.NotFound(
                new Error(
                    "Friends.NotFound",
                    "Friendship relationship not found."));
        }

        appDbContext.FriendRequests.Remove(friendship);

        await appDbContext.SaveChangesAsync();

        return Result.Success();
    }
}
