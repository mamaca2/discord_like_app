using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.InviteDTOs;
using DiscordApp.Application.DTOs.ChatDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Domain.Enums;
using DiscordApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services;

public class ServerInviteService(IAppDbContext dbContext) : IServerInviteService
{
    public async Task<Result<ServerInviteDto>> CreateInviteAsync(
        int serverId,
        string creatorUserId,
        CreateInviteDto dto)
    {
        var isOwner = await dbContext.ServerInfos
            .AnyAsync(s => s.Id == serverId && s.OwnerId == creatorUserId);

        if (!isOwner)
        {
            return Result<ServerInviteDto>.Failure(
                ServerInviteErrors.InvitationCreatorNotOwner);
        }

        var isMember = await dbContext.ServerMembers
            .AnyAsync(sm =>
                sm.ServerInfoId == serverId &&
                sm.UserId == creatorUserId);

        if (!isMember)
        {
            return Result<ServerInviteDto>.Failure(ServerErrors.NotMember);
        }

        var now = DateTime.UtcNow;

        var existingInvite = await dbContext.ServerInvites
            .FirstOrDefaultAsync(i =>
                i.ServerId == serverId &&
                i.CreatorId == creatorUserId &&
                i.ReceiverId == null &&
                (i.ExpiresAt == null || i.ExpiresAt > now) &&
                (i.MaxUses == 0 || i.Uses < i.MaxUses));

        if (existingInvite != null)
        {
            return Result<ServerInviteDto>.Success(existingInvite.ToDto());
        }

        var serverInvite = new ServerInvite
        {
            ServerId = serverId,
            CreatorId = creatorUserId,
            Code = Guid.NewGuid().ToString("N"),
            CreatedAt = now,
            ExpiresAt = dto.DurationHours.HasValue
                ? now.AddHours(dto.DurationHours.Value)
                : null,
            MaxUses = dto.MaxUses ?? 0,
            Uses = 0
        };

        dbContext.ServerInvites.Add(serverInvite);
        await dbContext.SaveChangesAsync();

        return Result<ServerInviteDto>.Success(serverInvite.ToDto());
    }

    public async Task<Result<DirectMessageResponseDto>> CreateInviteAndSendAsync(
        int serverId,
        string creatorUserId,
        string friendId,
        CreateInviteDto dto)
    {
        var result = await CreateInviteAsync(serverId, creatorUserId, dto);
        if (!result.IsSuccess)
        {
            return Result<DirectMessageResponseDto>.Failure(
                result.Errors![0]);
        }

        var invite = await dbContext.ServerInvites
            .FirstAsync(i => i.Code == result.Value!.Code);

        var message = new DirectMessage
        {
            SenderId = creatorUserId,
            ReceiverId = friendId,
            Content = string.Empty,
            SentAt = DateTime.UtcNow,
            ServerInviteId = invite.Id
        };

        dbContext.DirectMessages.Add(message);
        await dbContext.SaveChangesAsync();

        return Result<DirectMessageResponseDto>.Success(
            new DirectMessageResponseDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                SentAt = message.SentAt,
                InviteCode = invite.Code
            });
    }

    public async Task<Result<ServerDto>> AcceptInviteAsync(
        string code,
        string userId)
    {
        var invite = await dbContext.ServerInvites
            .Include(i => i.Server)
            .FirstOrDefaultAsync(i => i.Code == code);

        if (invite is null || invite.Server is null)
        {
            return Result<ServerDto>.NotFound(
                ServerInviteErrors.InvalidInviteCode);
        }

        if (invite.ReceiverId != null && invite.ReceiverId != userId)
        {
            return Result<ServerDto>.BadRequest(
                ServerInviteErrors.InvitationNotForYou);
        }

        if (invite.ExpiresAt.HasValue &&
            invite.ExpiresAt.Value <= DateTime.UtcNow)
        {
            return Result<ServerDto>.BadRequest(
                ServerInviteErrors.InviteExpired);
        }

        if (invite.MaxUses > 0 && invite.Uses >= invite.MaxUses)
        {
            return Result<ServerDto>.BadRequest(
                ServerInviteErrors.InviteMaxUsesReached);
        }

        var alreadyMember = await dbContext.ServerMembers
            .AnyAsync(m =>
                m.ServerInfoId == invite.ServerId &&
                m.UserId == userId);

        if (alreadyMember)
        {
            return Result<ServerDto>.BadRequest(
                ServerInviteErrors.AlreadyMember);
        }

        var newMember = new ServerMember
        {
            ServerInfoId = invite.ServerId,
            UserId = userId,
            Role = ServerRole.Member,
            JoinedAt = DateTime.UtcNow
        };

        dbContext.ServerMembers.Add(newMember);

        invite.Uses++;

        await dbContext.SaveChangesAsync();

        return Result<ServerDto>.Success(new ServerDto
        {
            Id = invite.Server.Id,
            Name = invite.Server.Name,
            Image = invite.Server.Image
        });
    }

    public Task<Result<List<ServerInviteDto>>> GetServerInvitesAsync(
        int serverId,
        string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RevokeInviteAsync(
        string code,
        string userId)
    {
        throw new NotImplementedException();
    }
}
