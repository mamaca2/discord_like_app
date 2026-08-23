using DiscordApp.constants;
using DiscordApp.contracts;
using DiscordApp.DTOs.FriendRequestDTO;
using DiscordApp.DTOs.FriendRequestDTOs;
using DiscordApp.Models;
using DiscordApp.Server.Models;
using DiscordApp.Server.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Services;

public class FriendRequestService: IFriendRequestService
{
    private AppDbContext _appDbContext;
    private UserManager<User> _userManager;
    public FriendRequestService(AppDbContext appDbContext, UserManager<User> userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
    }
    public Task<Result> AcceptAsync(int requestId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeclineAsync(int requestId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<FriendRequestDto>>> GetPendingRequestsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<FriendRequestDto>> SendAsync(string senderId, CreateFriendRequestDto dto)
    {
        if (senderId == dto.ReceiverId)
            return Result<FriendRequestDto>.BadRequest(
                new Error(ErrorCodes.BadRequest, "You cannot send a friend request to yourself."));

        var receiver = await _userManager.FindByIdAsync(dto.ReceiverId);
        if (receiver is null)
            return Result<FriendRequestDto>.NotFound(
                new Error(ErrorCodes.NotFound, "Receiver not found."));

        var existing = await _appDbContext.FriendRequests
            .AnyAsync(fr => fr.SenderId == senderId
                          && fr.ReceiverId == dto.ReceiverId
                          && fr.Status == FriendRequestStatus.Pending);

        if (existing)
            return Result<FriendRequestDto>.BadRequest(
                new Error(ErrorCodes.BadRequest, "A pending friend request already exists."));

        var sender = await _userManager.FindByIdAsync(senderId);

        var request = new FriendRequest
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            SentAt = DateTime.UtcNow,
            Status = FriendRequestStatus.Pending
        };
        _appDbContext.FriendRequests.Add(request);
        await _appDbContext.SaveChangesAsync();

        var result = new FriendRequestDto
        {
            Id = request.Id,
            SenderId = request.SenderId,
            SenderUserName = sender?.UserName ?? string.Empty,
            ReceiverId = request.ReceiverId,
            ReceiverUserName = receiver.UserName ?? string.Empty,
            SentAt = request.SentAt,
            Status = request.Status
        };

        return Result<FriendRequestDto>.Success(result);
    }
}
