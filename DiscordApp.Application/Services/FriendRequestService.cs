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


public class FriendRequestService: IFriendRequestService
{
    private IAppDbContext _appDbContext;
    private UserManager<User> _userManager;
    public FriendRequestService(IAppDbContext appDbContext, UserManager<User> userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
    }
    public async Task<Result> AcceptAsync(int requestId, string userId)
    {
        var request = await _appDbContext.FriendRequests
            .FirstOrDefaultAsync(fr => fr.Id == requestId);

        if (request is null)
            return Result.NotFound(
                new Error(ErrorCodes.NotFound, "Friend request not found or you are not authorized to accept it."));

        if (request.Status != FriendRequestStatus.Pending)
            return Result.BadRequest(
                new Error(ErrorCodes.BadRequest, "Only pending friend requests can be accepted."));

        request.Status = FriendRequestStatus.Accepted;
        request.RespondedAt = DateTime.UtcNow;

        await _appDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeclineAsync(int requestId, string userId)
    {
        var request = await _appDbContext.FriendRequests
            .FirstOrDefaultAsync(fr => fr.Id == requestId);

        if (request is null)
            return Result.NotFound(
                new Error(ErrorCodes.NotFound, "Friend request not found."));

        if (request.ReceiverId != userId && request.SenderId != userId)
            return Result.BadRequest(
                new Error(ErrorCodes.BadRequest, "You are not authorized to decline this friend request."));

        _appDbContext.FriendRequests.Remove(request);
        await _appDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<FriendRequestDto>>> GetIncomingPendingRequestsAsync(string userId)
    {
        var receiver = await _userManager.FindByIdAsync(userId);
        if (receiver is null)
            return Result<List<FriendRequestDto>>.NotFound(
                new Error(ErrorCodes.NotFound, "User not found."));

        var requests = await _appDbContext.FriendRequests
            .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.Pending)
            .Join(_appDbContext.Users,
                fr => fr.SenderId,
                sender => sender.Id,
                (fr, sender) => new FriendRequestDto
                {
                    Id = fr.Id,
                    SenderId = fr.SenderId,
                    SenderUserName = sender.UserName ?? string.Empty,
                    ReceiverId = fr.ReceiverId,
                    ReceiverUserName = receiver.UserName ?? string.Empty,
                    SentAt = fr.SentAt,
                    Status = fr.Status
                })
            .OrderByDescending(dto => dto.SentAt)
            .ToListAsync();

        return Result<List<FriendRequestDto>>.Success(requests);
    }

    public async Task<Result<List<FriendRequestDto>>> GetOutgoingPendingRequestsAsync(string userId)
    {
        var sender = await _userManager.FindByIdAsync(userId);
        if (sender is null)
            return Result<List<FriendRequestDto>>.NotFound(
                new Error(ErrorCodes.NotFound, "User not found."));

        var requests = await _appDbContext.FriendRequests
            .Where(fr => fr.SenderId == userId && fr.Status == FriendRequestStatus.Pending)
            .Join(_appDbContext.Users,
                fr => fr.ReceiverId,
                receiver => receiver.Id,
                (fr, receiver) => new FriendRequestDto
                {
                    Id = fr.Id,
                    SenderId = fr.SenderId,
                    SenderUserName = sender.UserName ?? string.Empty,
                    ReceiverId = fr.ReceiverId,
                    ReceiverUserName = receiver.UserName ?? string.Empty,
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

        var receiver = await _userManager.FindByIdAsync(dto.ReceiverId);
        Console.WriteLine($"receiver is null: {receiver is null}");
        if (receiver is null)
            return Result<FriendRequestDto>.NotFound(
                new Error(ErrorCodes.NotFound, "Receiver not found."));
        Console.WriteLine("------------- MOIDAAAAA!!!!!!!!! -------------------");
        // Check bi-directional relationship status
        var existingRelationship = await _appDbContext.FriendRequests
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
