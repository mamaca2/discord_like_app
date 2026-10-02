using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.Results;

namespace DiscordApp.Application.Interfaces;

public interface IServerService
{
    Task<Result<ServerDto>> CreateServerAsync(string userId, CreateServerDto dto);
    //Task<Result<ServerDto>> UpdateServerAsync(string serverId, string currentUserId, UpdateServerDto dto);
}