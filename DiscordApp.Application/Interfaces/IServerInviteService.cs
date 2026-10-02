using System;
using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.InviteDTOs;
using DiscordApp.Application.Results;

namespace DiscordApp.Application.Interfaces;

public interface IServerInviteService
{
    Task<Result<ServerInviteDto>> CreateInviteAsync(string serverId, string creatorUserId, CreateInviteDto dto);

    Task<Result<ServerDto>> AcceptInviteAsync(string code, string userId);

    Task<Result<List<ServerInviteDto>>> GetServerInvitesAsync(string serverId, string userId);

    Task<Result> RevokeInviteAsync(string code, string userId);
}
