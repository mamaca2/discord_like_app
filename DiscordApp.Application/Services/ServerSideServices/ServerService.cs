using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.Enums;
using DiscordApp.Domain.Models;

namespace DiscordApp.Application.Services;

public class ServerService(IAppDbContext dbContext) : IServerService
{
    public async Task<Result<ServerDto>> CreateServerAsync(string userId, CreateServerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<ServerDto>.BadRequest(
                new Error("InvalidInput", "Server name cannot be empty."));
        }

        var server = new ServerInfo
        {
            Name = dto.Name,
            Image = dto.Image,
            OwnerId = userId
        };

        var ownerMember = new ServerMember
        {
            Server = server,
            UserId = userId,
            Role = ServerRole.Owner
        };

        dbContext.ServerInfos.Add(server);
        dbContext.ServerMembers.Add(ownerMember);
        await dbContext.SaveChangesAsync();

        var response = new ServerDto
        {
            Id = server.Id,
            Name = server.Name,
            Image = server.Image,
            OwnerId = server.OwnerId,
            CreatedAt = server.CreatedAt,
            MemberCount = 1
        };

        return Result<ServerDto>.Success(response);
    }
}