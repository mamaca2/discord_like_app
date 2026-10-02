using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.InviteDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.Constants;
using DiscordApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services;

public class ServerInviteService(IAppDbContext dbContext) : IServerInviteService
{
    public async Task<Result<ServerInviteDto>> CreateInviteAsync(string serverId, string creatorUserId, CreateInviteDto dto)
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

    public Task<Result<ServerDto>> AcceptInviteAsync(string code, string userId)
    {
        throw new NotImplementedException();
    }

    
    public Task<Result<List<ServerInviteDto>>> GetServerInvitesAsync(string serverId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RevokeInviteAsync(string code, string userId)
    {
        throw new NotImplementedException();
    }
}