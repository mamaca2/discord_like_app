using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.Results;

namespace DiscordApp.Application.Interfaces;

public interface IServerService
{
    Task<Result<ServerDto>> CreateServerAsync(string userId, CreateServerDto dto);
    Task<Result<ServerDto>> UpdateServerAsync(string serverId, string currentUserId, UpdateServerDto dto);
    Task<Result<ServerDto>> GetServerAsync(string serverId);
    Task<Result<List<ServerDto>>> GetUserServersAsync(string userId);
    Task<Result> DeleteServerAsync(string serverId, string currentUserId);
}