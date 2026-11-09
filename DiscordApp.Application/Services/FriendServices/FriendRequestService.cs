using DiscordApp.Application.contracts;
using DiscordApp.Application.DTOs.FriendRequestDTO;
using DiscordApp.Application.DTOs.FriendRequestDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.Common;
using DiscordApp.Domain.constants;
using DiscordApp.Domain.Enums;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                new Error(ErrorCodes.NotFound, "Friend request not found or you are not authorized to accept it."));

        if (request.Status != FriendRequestStatus.Pending)
            return Result.BadRequest(
                new Error(ErrorCodes.BadRequest, "Only pending friend requests can be accepted."));

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
                new Error(ErrorCodes.NotFound, "Friend request not found."));

        if (request.ReceiverId != userId && request.SenderId != userId)
            return Result.BadRequest(
                new Error(ErrorCodes.BadRequest, "You are not authorized to decline this friend request."));

        dbContext.FriendRequests.Remove(request);
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<FriendRequestDto>>> GetIncomingPendingRequestsAsync(string userId)
    {
        var receiver = await userManager.FindByIdAsync(userId);
        if (receiver is null)
            return Result<List<FriendRequestDto>>.NotFound(
                new Error(ErrorCodes.NotFound, "User not found."));

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
                new Error(ErrorCodes.NotFound, "User not found."));

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
                new Error(ErrorCodes.BadRequest, "You cannot send a friend request to yourself."));

        var receiver = await userManager.FindByIdAsync(dto.ReceiverId);
        Console.WriteLine($"receiver is null: {receiver is null}");
        if (receiver is null)
            return Result<FriendRequestDto>.NotFound(
                new Error(ErrorCodes.NotFound, "Receiver not found."));
        // Check bi-directional relationship status
        var existingRelationship = await dbContext.FriendRequests
            .FirstOrDefaultAsync(fr =>
                (fr.SenderId == senderId && fr.ReceiverId == dto.ReceiverId) ||
                (fr.SenderId == dto.ReceiverId && fr.ReceiverId == senderId));

        if (existingRelationship is not null)
        {
            if (existingRelationship.Status == FriendRequestStatus.Accepted)
                return Result<FriendRequestDto>.BadRequest(
                    new Error(ErrorCodes.BadRequest, "You are already friends with this user."));

            if (existingRelationship.Status == FriendRequestStatus.Pending)
            {
                string message = existingRelationship.SenderId == senderId
                    ? "A pending friend request has already been sent to this user."
                    : "This user has already sent you a friend request. Accept their request instead.";

                return Result<FriendRequestDto>.BadRequest(
                    new Error(ErrorCodes.BadRequest, message));
            }
        }

        var sender = await userManager.FindByIdAsync(senderId);
                
        if (sender is null)
            return Result<FriendRequestDto>.NotFound(
                new Error(ErrorCodes.NotFound, "Sender not found."));

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
