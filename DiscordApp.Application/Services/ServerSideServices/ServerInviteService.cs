using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.InviteDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;

namespace DiscordApp.Application.Services;

public class ServerInviteService(IAppDbContext dbContext) : IServerInviteService
{
    public Task<Result<ServerDto>> AcceptInviteAsync(string code, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ServerInviteDto>> CreateInviteAsync(string serverId, string creatorUserId, CreateInviteDto dto)
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