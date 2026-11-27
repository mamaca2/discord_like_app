using DiscordApp.Application.DTOs.ChatDTOs;
using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.InviteDTOs;

namespace DiscordApp.Application.Interfaces;

public interface IServerInviteService
{
    Task<Result<ServerInviteDto>> CreateInviteAsync(int serverId, string creatorUserId, CreateInviteDto dto);
    Task<Result<ServerDto>> AcceptInviteAsync(string code, string userId);
    Task<Result<List<ServerInviteDto>>> GetServerInvitesAsync(int serverId, string userId);
    Task<Result> RevokeInviteAsync(string code, string userId);
    Task<Result<DirectMessageResponseDto>> CreateInviteAndSendAsync(
        int serverId,
        string creatorUserId,
        string friendId,
        CreateInviteDto dto);
}
