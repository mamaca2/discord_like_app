using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.ChatDTOs;

namespace DiscordApp.Application.Interfaces;

public interface IServerService
{
    Task<Result<ServerDto>> CreateServerAsync(string userId, CreateServerDto dto);
    Task<Result<ServerDto>> UpdateServerAsync(string serverId, string currentUserId, UpdateServerDto dto);
    Task<Result<List<ServerDto>>> GetUserServersAsync(string userId);
    Task<Result> DeleteServerAsync(int serverId, string ownerId);
    Task<Result<ServerChatDto>> CreateChatAsync(int serverId, string userId, CreateChatDto dto);
    Task<Result<List<ServerChatDto>>> GetChatAsync(int serverId, string userId);
    Task<Result<List<ServerChatMessageDto>>> GetServerChatMessagesAsync(
        int chatId,
        int serverId,
        string userId,
        int page = 1,
        int pageSize = 20);
    Task<Result<List<ServerMemberDto>>> GetServerMembersAsync(
        int serverId,
        string userId);
}