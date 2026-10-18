using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.InviteDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.constants;
using DiscordApp.Domain.Constants;
using DiscordApp.Domain.Enums;
using DiscordApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services;

public class ServerInviteService(IAppDbContext dbContext) : IServerInviteService
{
    public async Task<Result<ServerInviteDto>> CreateInviteAsync
    (
        int serverId, 
        string creatorUserId, 
        CreateInviteDto dto
    )
    {
        var isMember = await dbContext.ServerMembers
            .AnyAsync(sm => sm.ServerInfoId.Equals(serverId) && sm.UserId == creatorUserId);

        if (!isMember)
        {
            return Result<ServerInviteDto>.Failure(ServerErrors.NotMember);
        }

        var serverInvite = new ServerInvite
        {
            ServerId = serverId,
            CreatorId = creatorUserId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = dto.DurationHours.HasValue 
                ? DateTime.UtcNow.AddHours(dto.DurationHours.Value) 
                : null,
            MaxUses = dto.MaxUses ?? 0,
            Uses = 0
        };

        dbContext.ServerInvites.Add(serverInvite);
        await dbContext.SaveChangesAsync();

        return Result<ServerInviteDto>.Success(serverInvite.ToDto());
    }

    public async Task<Result<ServerDto>> AcceptInviteAsync(string code, string userId)
    {
        var invite = await dbContext.ServerInvites
            .Include(i => i.Server)
            .FirstOrDefaultAsync(i => i.Code == code);

        if (invite is null)
        {
            return Result<ServerDto>.NotFound(
                new Error(ErrorCodes.NotFound, "Invalid invite code."));
        }

        if (invite.ExpiresAt.HasValue && invite.ExpiresAt.Value < DateTime.UtcNow)
        {
            return Result<ServerDto>.BadRequest(
                new Error(ErrorCodes.BadRequest, "This invite code has expired."));
        }

        if (invite.MaxUses > 0 && invite.Uses >= invite.MaxUses)
        {
            return Result<ServerDto>.BadRequest(
                new Error(ErrorCodes.BadRequest, "This invite code has reached its maximum usage limit."));
        }

        var alreadyMember = await dbContext.ServerMembers
            .AnyAsync(m => m.ServerInfoId == invite.Server.Id && m.UserId == userId);

        if (alreadyMember)
        {
            return Result<ServerDto>.BadRequest(
                new Error(ErrorCodes.BadRequest, "You are already a member of this server."));
        }

        var newMember = new ServerMember
        {
            ServerInfoId = invite.Server.Id,
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

    public Task<Result<List<ServerInviteDto>>> GetServerInvitesAsync(int serverId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RevokeInviteAsync(string code, string userId)
    {
        throw new NotImplementedException();
    }
}