using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.Common;
using DiscordApp.Domain.constants;
using DiscordApp.Domain.Enums;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services;

public class ServerService(    
    IAppDbContext dbContext,
    IWebHostEnvironment environment
) : IServerService
{

    public async Task<Result<ServerDto>> CreateServerAsync(string userId, CreateServerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<ServerDto>.BadRequest(
                new Error("InvalidInput", "Server name cannot be empty."));
        }

        string? imagePath = null;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            if (dto.Image.Length > 5 * 1024 * 1024)
            {
                return Result<ServerDto>.BadRequest(
                    new Error("InvalidInput", "Image cannot exceed 5 MB."));
            }

            var extension = Path.GetExtension(dto.Image.FileName).ToLowerInvariant();

            if (extension != ".png" &&
                extension != ".jpg" &&
                extension != ".jpeg" &&
                extension != ".webp")
            {
                return Result<ServerDto>.BadRequest(
                    new Error("InvalidInput", "Unsupported image format."));
            }

            var webRoot = environment.WebRootPath
                ?? Path.Combine(environment.ContentRootPath, "wwwroot");

            var folder = Path.Combine(webRoot, "uploads", "servers");

            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folder, fileName);
            
            await using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await dto.Image.CopyToAsync(stream);
            }

            imagePath = $"/uploads/servers/{fileName}";
        }

        var server = new ServerInfo
        {
            Name = dto.Name,
            Image = imagePath,
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
            MemberCount = 1
        };

        return Result<ServerDto>.Success(response);
    }

    public async Task<Result> DeleteServerAsync(int serverId, string ownerId)
    {
        var server = await dbContext.ServerInfos
            .FirstOrDefaultAsync(s => s.Id == serverId);

        if (server is null)
        {
            return Result.NotFound(
                new Error(ErrorCodes.NotFound, "Server not found."));
        }

        if (server.OwnerId != ownerId)
        {
            return Result.BadRequest(
                new Error(ErrorCodes.BadRequest, "Only the server owner can delete this server."));
        }

        dbContext.ServerInfos.Remove(server);
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<ServerDto>>> GetUserServersAsync(string userId)
    {
        var servers = await dbContext.ServerMembers
            .AsNoTracking()
            .Where(sm => sm.UserId == userId)
            .Select(sm => new ServerDto
            {
                Id = sm.Server.Id,
                Name = sm.Server.Name,
                Image = sm.Server.Image,
                OwnerId = sm.Server.OwnerId,
                MemberCount = sm.Server.Members.Count
            })
            .ToListAsync();

        return Result<List<ServerDto>>.Success(servers);
    }

    public Task<Result<ServerDto>> UpdateServerAsync(string serverId, string currentUserId, UpdateServerDto dto)
    {
        throw new NotImplementedException();
    }
}