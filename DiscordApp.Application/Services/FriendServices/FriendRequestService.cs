using DiscordApp.Application.contracts;
using DiscordApp.Application.DTOs.FriendRequestDTO;
using DiscordApp.Application.DTOs.FriendRequestDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Domain.Enums;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services;

public class FriendRequestService(
    IAppDbContext dbContext,
    UserManager<User> userManager
) : IFriendRequestService
{
    public async Task<Result> AcceptAsync(int requestId, string userId)
    {
        var request = await dbContext.FriendRequests
            .FirstOrDefaultAsync(fr => fr.Id == requestId);

        if (request is null)
            return Result.NotFound(
                FriendRequestErrors.RequestNotFoundOrUnauthorized);

        if (request.Status != FriendRequestStatus.Pending)
            return Result.BadRequest(
                FriendRequestErrors.NotPending);

        request.Status = FriendRequestStatus.Accepted;
        request.RespondedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeclineAsync(int requestId, string userId)
    {
        var request = await dbContext.FriendRequests
            .FirstOrDefaultAsync(fr => fr.Id == requestId);

        if (request is null)
            return Result.NotFound(
                FriendRequestErrors.RequestNotFound);

        if (request.ReceiverId != userId && request.SenderId != userId)
            return Result.BadRequest(
                FriendRequestErrors.NotAuthorizedToDecline);

        dbContext.FriendRequests.Remove(request);
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<FriendRequestDto>>> GetIncomingPendingRequestsAsync(string userId)
    {
        var receiver = await userManager.FindByIdAsync(userId);

        if (receiver is null)
            return Result<List<FriendRequestDto>>.NotFound(
                FriendRequestErrors.UserNotFound);

        var requests = await dbContext.FriendRequests
            .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.Pending)
            .Join(dbContext.Users,
                fr => fr.SenderId,
                sender => sender.Id,
                (fr, sender) => new FriendRequestDto
                {
                    Id = fr.Id,
                    SenderId = fr.SenderId,
                    SenderUserName = sender.UserName ?? string.Empty,
                    SenderTag = sender.Tag,
                    SenderImage = sender.Image,

                    ReceiverId = fr.ReceiverId,
                    ReceiverUserName = receiver.UserName ?? string.Empty,
                    ReceiverTag = receiver.Tag,
                    ReceiverImage = receiver.Image,

                    SentAt = fr.SentAt,
                    Status = fr.Status
                })
            .OrderByDescending(dto => dto.SentAt)
            .ToListAsync();

        return Result<List<FriendRequestDto>>.Success(requests);
    }

    public async Task<Result<List<FriendRequestDto>>> GetOutgoingPendingRequestsAsync(string userId)
    {
        var sender = await userManager.FindByIdAsync(userId);

        if (sender is null)
            return Result<List<FriendRequestDto>>.NotFound(
                FriendRequestErrors.UserNotFound);

        var requests = await dbContext.FriendRequests
            .Where(fr => fr.SenderId == userId && fr.Status == FriendRequestStatus.Pending)
            .Join(dbContext.Users,
                fr => fr.ReceiverId,
                receiver => receiver.Id,
                (fr, receiver) => new FriendRequestDto
                {
                    Id = fr.Id,
                    SenderId = fr.SenderId,
                    SenderUserName = sender.UserName ?? string.Empty,
                    SenderTag = sender.Tag,
                    SenderImage = sender.Image,

                    ReceiverId = fr.ReceiverId,
                    ReceiverUserName = receiver.UserName ?? string.Empty,
                    ReceiverTag = receiver.Tag,
                    ReceiverImage = receiver.Image,

                    SentAt = fr.SentAt,
                    Status = fr.Status
                })
            .OrderByDescending(dto => dto.SentAt)
            .ToListAsync();

        return Result<List<FriendRequestDto>>.Success(requests);
    }

    public async Task<Result<FriendRequestDto>> SendAsync(string senderId, CreateFriendRequestDto dto)
    {
        if (senderId == dto.ReceiverId)
            return Result<FriendRequestDto>.BadRequest(
                FriendRequestErrors.CannotRequestYourself);

        var receiver = await userManager.FindByIdAsync(dto.ReceiverId);
        Console.WriteLine($"receiver is null: {receiver is null}");

        if (receiver is null)
            return Result<FriendRequestDto>.NotFound(
                FriendRequestErrors.ReceiverNotFound);

        // Check bi-directional relationship status
        var existingRelationship = await dbContext.FriendRequests
            .FirstOrDefaultAsync(fr =>
                (fr.SenderId == senderId && fr.ReceiverId == dto.ReceiverId) ||
                (fr.SenderId == dto.ReceiverId && fr.ReceiverId == senderId));

        if (existingRelationship is not null)
        {
            if (existingRelationship.Status == FriendRequestStatus.Accepted)
                return Result<FriendRequestDto>.BadRequest(
                    FriendRequestErrors.AlreadyFriends);

            if (existingRelationship.Status == FriendRequestStatus.Pending)
            {
                return Result<FriendRequestDto>.BadRequest(
                    existingRelationship.SenderId == senderId
                        ? FriendRequestErrors.AlreadySent
                        : FriendRequestErrors.IncomingRequestExists);
            }
        }

        var sender = await userManager.FindByIdAsync(senderId);

        if (sender is null)
            return Result<FriendRequestDto>.NotFound(
                FriendRequestErrors.SenderNotFound);

        var request = new FriendRequest
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            SentAt = DateTime.UtcNow,
            Status = FriendRequestStatus.Pending
        };

        dbContext.FriendRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var result = new FriendRequestDto
        {
            Id = request.Id,

            SenderId = request.SenderId,
            SenderUserName = sender?.UserName ?? string.Empty,
            SenderTag = sender.Tag ?? string.Empty,
            SenderImage = sender.Image,

            ReceiverId = request.ReceiverId,
            ReceiverUserName = receiver.UserName ?? string.Empty,
            ReceiverTag = receiver.Tag ?? string.Empty,
            ReceiverImage = receiver.Image,

            SentAt = request.SentAt,
            Status = request.Status
        };

        return Result<FriendRequestDto>.Success(result);
    }
}
