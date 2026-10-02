using DiscordApp.Application.contracts;
using DiscordApp.Application.DTOs.FriendDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.Common;
using DiscordApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordApp.Application.Services;

public class FriendsService(IAppDbContext appDbContext) : IFriendsService
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
