
using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.ChatDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.Constants;
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
            return Result<ServerDto>.BadRequest(ServerErrors.InvalidServerName);
        }

        string? imagePath = null;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            if (dto.Image.Length > 5 * 1024 * 1024)
            {
                return Result<ServerDto>.BadRequest(ServerErrors.ImageTooLarge);
            }

            var extension = Path.GetExtension(dto.Image.FileName).ToLowerInvariant();

            if (extension != ".png" &&
                extension != ".jpg" &&
                extension != ".jpeg" &&
                extension != ".webp")
            {
                return Result<ServerDto>.BadRequest(ServerErrors.UnsupportedImageFormat);
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
            return Result.NotFound(ServerErrors.ServerNotFound);
        }

        if (server.OwnerId != ownerId)
        {
            return Result.BadRequest(ServerErrors.DeleteServerNotOwner);
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

    public async Task<Result<ServerChatDto>> CreateChatAsync(int serverId, string userId, CreateChatDto dto)
    {
        var server = await dbContext.ServerInfos
            .FirstOrDefaultAsync(s => s.Id == serverId);

        if (server is null)
        {
            return Result<ServerChatDto>.NotFound(ServerErrors.ServerNotFound);
        }

        if (server.OwnerId != userId)
        {
            return Result<ServerChatDto>.BadRequest(ServerErrors.CreateChatNotOwner);
        }

        var chat = new ServerChat
        {
            Name = dto.Name,
            ServerId = serverId,
            Server = server
        };

        dbContext.ServerChats.Add(chat);
        await dbContext.SaveChangesAsync();

        var serverChatdto = new ServerChatDto
        {
            Id = chat.Id,
            Name = dto.Name,
            ServerId = serverId
        };

        return Result<ServerChatDto>.Success(serverChatdto);
    }

    public async Task<Result<List<ServerChatDto>>> GetChatAsync(int serverId, string userId)
    {
        var server = await dbContext.ServerInfos
            .FirstOrDefaultAsync(s => s.Id == serverId);

        if (server is null)
        {
            return Result<List<ServerChatDto>>.NotFound(ServerErrors.ServerNotFound);
        }

        var isMember = await dbContext.ServerMembers
            .AnyAsync(m => m.ServerInfoId == serverId && m.UserId == userId);

        if (!isMember)
        {
            return Result<List<ServerChatDto>>.BadRequest(ServerErrors.NotMember);
        }

        var chats = await dbContext.ServerChats
            .Where(s => s.ServerId == serverId)
            .Select(s => new ServerChatDto
            {
                Id = s.Id,
                Name = s.Name,
                ServerId = s.ServerId
            })
            .ToListAsync();

        return Result<List<ServerChatDto>>.Success(chats);
    }

    public async Task<Result<List<ServerChatMessageDto>>> GetServerChatMessagesAsync(
        int chatId,
        int serverId,
        string userId,
        int page = 1,
        int pageSize = 20)
    {
        if (page < 1 || pageSize < 1)
        {
            return Result<List<ServerChatMessageDto>>.BadRequest(ServerErrors.InvalidPagination);
        }

        var server = await dbContext.ServerInfos
            .AnyAsync(s => s.Id == serverId);

        if (!server)
        {
            return Result<List<ServerChatMessageDto>>.BadRequest(ServerErrors.ServerNotFound);
        }

        var isMember = await dbContext.ServerMembers
            .AnyAsync(m => m.ServerInfoId == serverId && m.UserId == userId);

        if (!isMember)
        {
            return Result<List<ServerChatMessageDto>>.BadRequest(ServerErrors.NotMember);
        }

        var isChatInServer = await dbContext.ServerChats
            .AnyAsync(c => c.ServerId == serverId && c.Id == chatId);

        if (!isChatInServer)
        {
            return Result<List<ServerChatMessageDto>>.BadRequest(ServerErrors.ChatNotInServer);
        }

        var messages = await dbContext.ServerChatMessages
            .Where(m => m.ServerChatId == chatId)
            .OrderByDescending(m => m.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new ServerChatMessageDto
            {
                Id = m.Id,
                ChatId = m.ServerChatId,
                SenderId = m.SenderId,
                Content = m.Content,
                IsMine = m.SenderId == userId
            })
            .ToListAsync();

        messages.Reverse();

        return Result<List<ServerChatMessageDto>>.Success(messages);
    }


    public async Task<Result<List<ServerMemberDto>>> GetServerMembersAsync(
        int serverId,
        string userId)
    {
        var isMember = await dbContext.ServerMembers
            .AnyAsync(m =>
                m.ServerInfoId == serverId &&
                m.UserId == userId);

        if (!isMember)
            return Result<List<ServerMemberDto>>.BadRequest(
                ServerErrors.NotMember);

        var members = await dbContext.ServerMembers
            .AsNoTracking()
            .Where(m => m.ServerInfoId == serverId)
            .Join(dbContext.Users,
                m => m.UserId,
                u => u.Id,
                (m, u) => new
                {
                    u.Id,
                    u.DisplayName,
                    u.UserName,
                    u.Image,
                    m.Role
                })
            .ToListAsync();

        return Result<List<ServerMemberDto>>.Success(
            members.Select(m => new ServerMemberDto
            {
                Id = m.Id,
                UserName = m.DisplayName ?? m.UserName ?? string.Empty,
                Image = m.Image,
                Role = m.Role.ToString()
            }).ToList());
    }

    public Task<Result<ServerDto>> UpdateServerAsync(string serverId, string currentUserId, UpdateServerDto dto)
    {
        throw new NotImplementedException();
    }
}
